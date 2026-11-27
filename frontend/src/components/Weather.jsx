import { getWeather } from "../api/weather.js";
import useApiResource from "../hooks/useApiResource.js";
import ResourceStatus from "./ResourceStatus.jsx";

export default function Weather() {
  const { data, status, error, reload } = useApiResource(getWeather, "weather");
  return <section aria-labelledby="weather-title">
    <p className="eyebrow">PUBLIC API</p><h2 id="weather-title">Weather in The Hague</h2>
    <ResourceStatus status={status} error={error} onRetry={reload} label="weather" />
    {status === "success" && <p><strong>{Math.round(data.temperature_2m)} °C</strong> · Humidity {data.relative_humidity_2m}%</p>}
    <p className="hint">Weather data by <a href="https://open-meteo.com/" target="_blank" rel="noreferrer">Open-Meteo</a>.</p>
  </section>;
}
