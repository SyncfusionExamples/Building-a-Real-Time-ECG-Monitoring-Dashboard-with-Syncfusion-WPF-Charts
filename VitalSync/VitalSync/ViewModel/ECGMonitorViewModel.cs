using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace VitalSync.ViewModel
{
    public partial class ECGMonitorViewModel : INotifyPropertyChanged
    {
        private bool _isTimerRunning;
        private static int _index;
        private static float _time;
        private int _tickCounter = 0;
        private const int TicksPerSecond = 250;

        private ObservableCollection<ECGDataPoint>? _liveData;

        public string? HeartRate { get; set; }
        public  string? BloodPressure { get; set; }
        public  string? BodyTemperature { get; set; }
        public  string? PRInterval { get; set; }
        public  string? QTInterval { get; set; }
        public  string? QRSDuration { get; set; }
        private Random _random = new Random();

        private DispatcherTimer? _timer;
        public ObservableCollection<ECGDataPoint>? LiveData
        {
            get => _liveData;
            set
            {
                if (_liveData != value)
                {
                    _liveData = value;
                    OnPropertyChanged(nameof(LiveData));
                }
            }
        }


        readonly float[] _ecgSignal =
        [
             0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,
             0.000000f,     0.000004f,     0.000039f,     0.000315f,     0.001832f,     0.007730f,     0.023693f,     0.052729f,
             0.085214f,     0.100000f,     0.085214f,     0.052729f,     0.023693f,     0.007730f,     0.001832f,     0.000315f,
             0.000039f,     0.000004f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     -0.000000f,
             -0.000000f,     -0.000000f,     -0.000000f,     -0.000001f,     -0.000019f,     -0.000289f,     -0.002745f,     -0.015774f,
             -0.054788f,     -0.113669f,     -0.131684f,     -0.039515f,     0.181746f,     0.511481f,     0.849366f,     0.999228f,
             0.847546f,     0.500942f,     0.144958f,     -0.117395f,     -0.231684f,     -0.191548f,     -0.091573f,     -0.026307f,
             -0.004560f,     -0.000445f,     0.000050f,     0.000170f,     0.000348f,     0.000683f,     0.001296f,     0.002372f,
             0.004190f,     0.007142f,     0.011749f,     0.018653f,     0.028579f,     0.042258f,     0.060300f,     0.083041f,
             0.110364f,     0.141553f,     0.175213f,     0.209303f,     0.241291f,     0.268452f,     0.288237f,     0.298670f,
             0.298670f,     0.288237f,     0.268452f,     0.241291f,     0.209303f,     0.175213f,     0.141553f,     0.110364f,
             0.083041f,     0.060300f,     0.042258f,     0.028579f,     0.018653f,     0.011749f,     0.007142f,     0.004190f,
             0.002372f,     0.001296f,     0.000683f,     0.000348f,     0.000171f,     0.000081f,     0.000037f,     0.000016f,
             0.000007f,     0.000003f,     0.000001f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,
             0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,
             0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f,     0.000000f
        ];


        public ECGMonitorViewModel()
        {
            LiveData = [];
            InitializeTimer();
            LoadInitialData();
            StartTimer();
            InitializeVitalSignsData();
        }

        // Initialize default values for vital signs
        private void InitializeVitalSignsData()
        {
            HeartRate = "72 bpm";
            BloodPressure = "120/80 mmHg";
            BodyTemperature = "36.7 °C";
            PRInterval = "160 ms";
            QTInterval = "360 ms";
            QRSDuration = "90 ms";
        }

        // Setup the timer for updating ECG data
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(4) // 250Hz sampling rate
            };
            _timer.Tick += (s, e) => UpdateData();
        }

        // Load initial ECG data points into the collection
        private void LoadInitialData()
        {
            LiveData?.Clear();
            _index = 0;
            _time = 0f;

            for (int i = 0; i < 50; i++)
            {
                AddNextDataPoint();
            }
        }

        // Update ECG data and refresh vital signs periodically
        private void UpdateData()
        {
            if (!_isTimerRunning) return;

            _tickCounter++;

            if (LiveData?.Count > 250)
                LiveData.RemoveAt(0);

            if (_tickCounter >= TicksPerSecond)
            {
                _tickCounter = 0;

                // Randomly adjust the heart rate and intervals for simulation
                HeartRate = _random.Next(68, 75).ToString() + " bpm";
                PRInterval = _random.Next(120, 200).ToString() + " ms"; // ms
                QTInterval = _random.Next(350, 450).ToString() + " ms"; // ms
                QRSDuration = _random.Next(80, 120).ToString() + " ms"; // ms

                // Notify changes in the properties
                OnPropertyChanged(nameof(HeartRate));
                OnPropertyChanged(nameof(PRInterval));
                OnPropertyChanged(nameof(QTInterval));
                OnPropertyChanged(nameof(QRSDuration));
            }
            AddNextDataPoint();
        }

        // Add the next point in the ECG signal to the live data
        private void AddNextDataPoint()
        {
            if (_index >= _ecgSignal.Length)
                _index = 0;

            var voltage = _ecgSignal[_index];
            LiveData?.Add(new ECGDataPoint(_time, voltage));

            _index++;
            _time += 0.004f; // 4ms per point = 250Hz
        }

        // Start the timer to begin updating ECG data
        public void StartTimer()
        {
            _isTimerRunning = true;
            _timer?.Start();
        }

        // Stop the timer to cease updating ECG data
        public void StopTimer()
        {
            _isTimerRunning = false;
            _timer?.Stop();
        }

        // Reset the timer and reload initial ECG data
        public void Reset()
        {
            StopTimer();
            LoadInitialData();
            StartTimer();
        }

        // Notify that a property has changed

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
