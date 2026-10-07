using System;

namespace WeatherStationApp
{
    public class WeatherChangedEventArgs : EventArgs
    {
        public double Temperature { get; }
        public double Humidity { get; }

        public WeatherChangedEventArgs(double temperature, double humidity)
        {
            Temperature = temperature;
            Humidity = humidity;
        }
    }

    public class WeatherStation
    {
        private double _temperature;
        private double _humidity;

        public event EventHandler<WeatherChangedEventArgs>? WeatherChanged;

        public double Temperature => _temperature;
        public double Humidity => _humidity;

        public void SetMeasurements(double newTemperature, double newHumidity)
        {
            if (Math.Abs(_temperature - newTemperature) > 0.01 || Math.Abs(_humidity - newHumidity) > 0.01)
            {
                _temperature = newTemperature;
                _humidity = newHumidity;
                OnWeatherChanged(new WeatherChangedEventArgs(_temperature, _humidity));
            }
        }

        protected virtual void OnWeatherChanged(WeatherChangedEventArgs e)
        {
            WeatherChanged?.Invoke(this, e);
        }
    }

    public class DisplayScreen
    {
        public void OnWeatherChanged(object? sender, WeatherChangedEventArgs e)
        {
            Console.WriteLine($"[Дисплей] Оновлення: Температура = {e.Temperature:F1}°C, Вологість = {e.Humidity:F1}%");
        }
    }

    public class PhoneAlertService
    {
        public void OnWeatherChanged(object? sender, WeatherChangedEventArgs e)
        {
            Console.WriteLine($"[Телефон] Push-сповіщення: Погода змінилася! ({e.Temperature:F1}°C / {e.Humidity:F1}%)");
        }
    }
}