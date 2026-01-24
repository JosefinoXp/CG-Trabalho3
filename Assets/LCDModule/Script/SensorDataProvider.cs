using UnityEngine;
using System;

namespace LCDModule
{
    /// <summary>
    /// Provides sensor data (temperature and humidity) and notifies listeners when the values change.
    /// </summary>
    public class SensorDataProvider : MonoBehaviour
    {
        /// <summary>
        /// Invoked when temperature or humidity changes.
        /// </summary>
        public event Action<float, float> OnDataChanged; // Triggered when temperature or humidity changes

        [Range(-20f, 50f)] public float temperature = 25f; // Adjustable via Inspector slider
        [Range(0f, 100f)] public float humidity = 50f;     // Adjustable via Inspector slider

        private float prevTemperature; // Previous temperature value
        private float prevHumidity;    // Previous humidity value

        /// <summary>
        /// Gets or sets the current temperature value.
        /// Invokes <see cref="OnDataChanged"/> when the value changes significantly.
        /// </summary>
        public float Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                OnDataChanged?.Invoke(temperature, humidity);
            }
        }

        /// <summary>
        /// Gets or sets the current humidity value.
        /// Invokes <see cref="OnDataChanged"/> when the value changes significantly.
        /// </summary>
        public float Humidity
        {
            get => humidity;
            set
            {
                humidity = value;
                OnDataChanged?.Invoke(temperature, humidity);
            }
        }

        private void Update()
        {
            // Reflects changes made in the Inspector and triggers an update event if values have changed
            if (Mathf.Abs(prevTemperature - temperature) > 0.01f ||
                Mathf.Abs(prevHumidity - humidity) > 0.01f)
            {
                OnDataChanged?.Invoke(temperature, humidity);
                prevTemperature = temperature;
                prevHumidity = humidity;
            }
        }
    }
}