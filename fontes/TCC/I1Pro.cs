using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace i1Sharp
{

    public class I1Pro : IDisposable
    {
        private static readonly string I1_MEASUREMENT_MODE = "MeasurementMode";
        private static readonly string I1_REFLECTANCE_SCAN = "ReflectanceScan";
        
        public delegate void DeviceConnectedHandler(I1Pro device);
        public static event DeviceConnectedHandler DeviceConnected;
        
        public delegate void DeviceDisconnectedHandler(I1Pro device);
        public static event DeviceDisconnectedHandler DeviceDisconnected;
        
        public delegate void ButtonPressedHandler(I1Pro device);
        public event ButtonPressedHandler ButtonPressed;
        
        public delegate void ScanReadyHandler(I1Pro device);
        public event ScanReadyHandler ScanReady;
        
        public delegate void LampRestoredHandler(I1Pro device);
        public event LampRestoredHandler LampRestored;
        
        public delegate void LogEventHandler(LogType type, string logEntry);

        public static event LogEventHandler LogEvent;

        public static int SpectrumSize { get { return 36; } }

        private static bool sdkInitialized = false;

        private static Dictionary<IntPtr, I1Pro> deviceMap = new Dictionary<IntPtr, I1Pro>();

        private static DeviceEventCallback callbackFunction;



        private IntPtr Handle { get; set; }

        public string SerialNumber { get; private set; }

        public string DeviceType { get; private set; }

        public bool IsOpen { get; private set; }

        public enum Result
        {
            eNoError = 0,     //sem erro
            eException = 1,     //excessao interna
            eBadBuffer = 2,     //o tamanho do buffer não é grande o suficiente para os dados
            eInvalidHandle = 9,     //dispositivo desconectado
            eInvalidArgument = 10,     //um argumento do método passado é inválido
            eDeviceNotOpen = 11,     //o dispositivo não está aberto
            eDeviceNotConnected = 12,     //o dispositivo não está fisicamente ligado ao computador
            eDeviceNotCalibrated = 13,     //o dispositivo não foi calibrado ou a calibração expirou
            eNoDataAvailable = 14,     //no modo de varredura
            eNoMeasureModeSet = 15,     //nenhum modo de medida foi definido
            eNoReferenceChartLine = 17,     //nenhuma linha de gráfico de referência para o conjunto de correlações
            eNoSubstrateWhite = 18,     //sem conjunto de referência branco de substrato
            eNotLicensed = 19,     //não disponível para este dispositivo
            eDeviceAlreadyOpen = 20,     //o dispositivo já foi aberto
            eDeviceAlreadyInUse = 51,     //o dispositivo já está sendo usado por outro aplicativ
            eDeviceCommunicationError = 52,     //um erro de comunicação USB, tente reconectar o dispositivo
            eUSBPowerProblem = 53,     //um problema de energia USB foi detectado
            eNotOnWhiteTile = 54,     //calibragem falhou porque o dispositivo pode não estar em branco ou o controle deslizante de azulejo branco está fechado

            eStripRecognitionFailed = 60,     //o reconhecimento falhou. Digitalizar novamente
            eChartCorrelationFailed = 61,     //não pôde mapear dados digitalizados para o gráfico de referência
            eInsufficientMovement = 62,     //distância de movimento muito curto na régua i1Pro2
            eExcessiveMovement = 63,     //a distância de movimento excessiva e excede a régua licenciada i1Pro2
            eEarlyScanStart = 64,     //correções perdidas no início de uma varredura
            eUserTimeout = 65,     //ação do usuário demorou muito, tente novamente mais rápido
            eIncompleteScan = 66,     //usuário não varreu todos os patches
            eDeviceNotMoved = 67,     //usuário não se moveu durante a medição de varredura

            eDeviceCorrupt = 71,     //um diagnóstico interno detectou um problema com os instrumentos dados
            eWavelengthShift = 72      //um diagnóstico interno de mudança de comprimento de onda detectou um problema com sensor espectral
        }
        
        private enum I1Event
        {
            eI1ProArrival         = 0x11,  //< i1Pro conectado
            eI1ProDeparture       = 0x12,  //< i1Pro desconectado 

            eI1ProButtonPressed   = 0x01,
            eI1ProScanReadyToMove = 0x02,  
            eI1ProLampRestore     = 0x03
        }
        
        public enum MeasurementModeType
        {
            eUndefined,
            eReflectanceScan
        }

        public enum LogType
        {
            eNormal,
            eError
        }
        
        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_GetDevices ([MarshalAs (UnmanagedType.LPArray)] ref IntPtr [] devices, ref ulong count);
        
        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_OpenDevice (IntPtr handle);
        
        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_CloseDevice (IntPtr handle);

       [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_SetOption (IntPtr handle, string key, string value);
        
        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_GetOption (IntPtr handle, string key, StringBuilder buffer, ref uint bufferSize);
        
        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_Calibrate (IntPtr handle);

        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_TriggerMeasurement (IntPtr handle);

        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int I1_GetNumberOfAvailableSamples (IntPtr handle);

        [DllImport("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result I1_GetTriStimulus(IntPtr handle, float[] tristimulus, int index);

        [DllImport("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result SetReferenceChartLine(IntPtr handle, float contRREFENCE, int index);
        
        public delegate void DeviceEventCallback (IntPtr device, uint eventId, IntPtr context);

        [DllImport ("i1Pro64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DeviceEventCallback I1_RegisterDeviceEventHandler (DeviceEventCallback deviceEventFunction, IntPtr context);
        
        public static void SetupSDK ()
        {
            if (!sdkInitialized)
            {
                callbackFunction = new DeviceEventCallback (DeviceEventHandler);
                IntPtr context = new IntPtr ();
                DeviceEventCallback oldCallback = I1_RegisterDeviceEventHandler (callbackFunction, context);

                sdkInitialized = true;
            }
        } 

        private static void WriteLog (LogType type, string logEntry)
        {
            if (LogEvent != null)
            {
                LogEvent (type, logEntry);
            }
        }
        
        private static void DeviceEventHandler (IntPtr handle, uint eventId, IntPtr context)
        {
            switch ((I1Event) eventId)
            {
                case I1Event.eI1ProArrival: 
                    try
                    {
                        I1Pro newDevice = CreateDevice (handle);
                        {
                            WriteLog (LogType.eNormal, "Dispositivo conectado");
                            if (DeviceConnected != null)
                            {
                                DeviceConnected (newDevice);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        WriteLog(LogType.eNormal, "Erro ao conectar o dispositivo");
                    }
                    break;
                default:
                    if (deviceMap.ContainsKey (handle))
                    {
                        I1Pro device = deviceMap [handle];
                        switch ((I1Event)eventId)
                        {
                            case I1Event.eI1ProDeparture:
                                WriteLog (LogType.eNormal, "Dispositivo desconectado");
                                deviceMap.Remove (handle);
                                if (DeviceDisconnected != null)
                                {
                                    DeviceDisconnected (device);
                                }
                                break;
                            case I1Event.eI1ProButtonPressed:
                                if (device.ButtonPressed != null)
                                {
                                    device.ButtonPressed (device);
                                }
                                break;
                            case I1Event.eI1ProScanReadyToMove:
                                if (device.ScanReady != null)
                                {
                                    device.ScanReady (device);
                                }
                                break;
                            case I1Event.eI1ProLampRestore: 
                                if (device.LampRestored != null)
                                {
                                    device.LampRestored (device);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public static List<I1Pro> LoadDevices ()
        {
            SetupSDK ();
            deviceMap.Clear ();
            ulong count = 0;
            IntPtr [] deviceHdlArray = null;
            Result result = I1Pro.I1_GetDevices (ref deviceHdlArray, ref count);
            HandleResult (result);
            List<I1Pro> devices = new List<I1Pro> ();
            if (count > 0)
            {
                foreach (var deviceHandle in deviceHdlArray)
                {
                    try
                    {
                        devices.Add (CreateDevice (deviceHandle));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            WriteLog (LogType.eNormal, String.Format ("Dispositivo encontrado: {0}", devices.Count));
            return devices;
        }
        
        private static I1Pro CreateDevice (IntPtr deviceHandle)
        {
            using (I1Pro device = new I1Pro (deviceHandle))
            {
                device.Open ();
                deviceMap.Add (deviceHandle, device);
                return device;
            }
        }
        
        private static void HandleResult (Result result)
        {
            if (result == Result.eNoError)
            {
                return;
            }

            WriteLog (LogType.eError, String.Format ("Erro: {0}", result));
            switch (result)
            {
                case Result.eNoError:                  return;
                case Result.eException:                throw new I1ProException (result, "Excessao interna");
                case Result.eBadBuffer:                throw new I1ProException (result, "O tamanho do buffer não é grande o suficiente para os dados");
                case Result.eInvalidHandle:            throw new I1ProException (result, "Dispositivo desconectado");
                case Result.eInvalidArgument:          throw new I1ProException (result, "Um argumento do método passado é inválido");
                case Result.eDeviceNotOpen:            throw new I1ProException (result, "O dispositivo não está aberto");
                case Result.eDeviceNotConnected:       throw new I1ProException (result, "O dispositivo não está fisicamente ligado ao computador");
                case Result.eDeviceNotCalibrated:      throw new I1ProException (result, "O dispositivo não foi calibrado ou a calibração expirou");
                case Result.eNoDataAvailable:          throw new I1ProException (result, "No modo de varredura");
                case Result.eNoMeasureModeSet:         throw new I1ProException (result, "Nenhum modo de medida foi definido");
                case Result.eNoReferenceChartLine:     throw new I1ProException (result, "Nenhuma linha de gráfico de referência para o conjunto de correlações");
                case Result.eNoSubstrateWhite:         throw new I1ProException (result, "Sem conjunto de referência branco de substrato");
                case Result.eNotLicensed:              throw new I1ProException (result, "Não disponível para este dispositivo");
                case Result.eDeviceAlreadyOpen:        throw new I1ProException (result, "O dispositivo já foi aberto");

                case Result.eDeviceAlreadyInUse:       throw new I1ProException (result, "O dispositivo já está sendo usado por outro aplicativo");
                case Result.eDeviceCommunicationError: throw new I1ProException (result, "Um erro de comunicação USB, tente reconectar o dispositivo");
                case Result.eUSBPowerProblem:          throw new I1ProException (result, "Um problema de energia USB foi detectado");
                case Result.eNotOnWhiteTile:           throw new I1ProException (result, "Calibragem falhou porque o dispositivo pode não estar em branco ou o controle deslizante de azulejo branco está fechado");

                case Result.eStripRecognitionFailed:   throw new I1ProException (result, "O reconhecimento falhou. Digitalizar novamente");
                case Result.eChartCorrelationFailed:   throw new I1ProException (result, "Não pôde mapear dados digitalizados para o gráfico de referência");
                case Result.eInsufficientMovement:     throw new I1ProException (result, "Distância de movimento muito curto na régua i1Pro2");
                case Result.eExcessiveMovement:        throw new I1ProException (result, "A distância de movimento excessiva e excede a régua licenciada i1Pro2");
                case Result.eEarlyScanStart:           throw new I1ProException (result, "Correções perdidas no início de uma varredura");
                case Result.eUserTimeout:              throw new I1ProException (result, "Ação do usuário demorou muito, tente novamente mais rápido");
                case Result.eIncompleteScan:           throw new I1ProException (result, "Usuário não varreu todos os patches");
                case Result.eDeviceNotMoved:           throw new I1ProException (result, "Usuário não se moveu durante a medição de varredura");

                case Result.eDeviceCorrupt:            throw new I1ProException (result, "Um diagnóstico interno detectou um problema com os instrumentos dados");
                case Result.eWavelengthShift:          throw new I1ProException (result, "Um diagnóstico interno de mudança de comprimento de onda detectou um problema com sensor espectral");

                default:                               throw new I1ProException (result, "Erro desconhecido");
            }
        }

        private static Dictionary<MeasurementModeType, string> measurementModeDictionary;
        
        private static Dictionary<MeasurementModeType, string> MeasurementModeDictionary
        {
            get
            {
                if (measurementModeDictionary == null)
                {
                    measurementModeDictionary = new Dictionary<MeasurementModeType, string> ();
                    measurementModeDictionary.Add (MeasurementModeType.eReflectanceScan, I1_REFLECTANCE_SCAN);
              }
                return measurementModeDictionary;
            }
        }
        
        public string Name 
        {
            get
            {
                StringBuilder nameBuilder = new StringBuilder ();
                nameBuilder.Append (DeviceType);
                if (SerialNumber != "")
                {
                    if (nameBuilder.Length > 0)
                    {
                        nameBuilder.Append (" - ");
                    }
                    nameBuilder.Append (SerialNumber);
                }

                if (nameBuilder.Length == 0)
                {
                    nameBuilder.Append (Handle.ToString ());
                }

                return nameBuilder.ToString ();
            }
        }
        
        private I1Pro (IntPtr handle)
        {
            if (handle.ToInt32 () == 0)
            {
                throw new I1ProException (Result.eInvalidHandle, "Invalid handle");
            }
            this.Handle = handle;
            this.DeviceType = "";
            this.SerialNumber = "";
            this.IsOpen = false;
        }
        
        public void Dispose ()
        {
            Close ();
        }
        
        public void Open ()
        {
            Result result = I1_OpenDevice (Handle);
            HandleResult (result);
            IsOpen = true;
            DeviceType = GetOption ("DeviceTypeKey");
            SerialNumber = GetOption ("SerialNumber");
        }
        
        public void Close ()
        {
            if (this.IsOpen)
            {
                Result result = I1_CloseDevice (Handle);
                HandleResult (result);
                IsOpen = false;
            }
        }
        
        private string GetOption (string key)
        {
            StringBuilder buffer = new StringBuilder (100);
            UInt32 bufferSize = (UInt32)buffer.Capacity;
            Result result = I1_GetOption (Handle, key, buffer, ref bufferSize);
            if (result == Result.eBadBuffer)
            {
                buffer.Capacity = (int)bufferSize;
                result = I1_GetOption (Handle, key, buffer, ref bufferSize);
            }
            HandleResult (result);
            return buffer.ToString ();
        }
        
        private void SetOption (string key, string value)
        {
            Result result = I1_SetOption (Handle, key, value);
            HandleResult (result);
        }
        
        private MeasurementModeType MeasurementModeFromString (String measurementModeString)
        {
            if (MeasurementModeDictionary.ContainsValue (measurementModeString))
            {
                return MeasurementModeDictionary.FirstOrDefault (x => x.Value == measurementModeString).Key;
            }
            return MeasurementModeType.eUndefined;
        }
        
        public MeasurementModeType MeasurementMode
        {
            get
            {
                string measurementMode = GetOption (I1_MEASUREMENT_MODE);
                return MeasurementModeFromString (measurementMode);
            }

            set
            {
                if (!MeasurementModeDictionary.ContainsKey (value))
                {
                    throw new I1ProException (Result.eInvalidArgument, "Modo de medição desconhecido");
                }
                SetOption (I1_MEASUREMENT_MODE, MeasurementModeDictionary [value]);
            }
        }
        
        public void Calibrate (MeasurementModeType measurementMode = MeasurementModeType.eReflectanceScan)
        {
            MeasurementMode = measurementMode;
            Result result = I1_Calibrate (Handle);
            HandleResult (result);
            WriteLog (LogType.eNormal, "Dispositivo calibrado");
        }
        
        public void TriggerMeasurement (MeasurementModeType measurementMode)
        {
            MeasurementMode = measurementMode;
            Result result = I1_TriggerMeasurement (Handle);
            HandleResult (result);
        }
        
        public int SampleCount
        {
            get
            {
                int availableSampleCount = I1_GetNumberOfAvailableSamples (Handle);
                return availableSampleCount;
            }
        }
        
        public float [] GetSample (int index = 0)
        {
            float[] color = new float[SpectrumSize];
            Result result2 = I1_GetTriStimulus(Handle, color, 0);
            HandleResult(result2);

            return color;
        }
        
        public List<float []> Samples {
            get
            {
                List <float[]> samples = new List<float[]> ();
                for (int i = 0; i < SampleCount; ++i)
                {
                    samples.Add (GetSample (i));
                }
                return samples;
            }
        }
    }
}
