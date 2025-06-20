namespace VitalSync.ViewModel
{
    public class ECGDataPoint
    {
        public float Time { get; set; }      // Time in seconds 
        public float Voltage { get; set; }   // ECG voltage in millivolts

        public ECGDataPoint(float time, float voltage)
        {
            Time = time;
            Voltage = voltage;
        }
    }

}
