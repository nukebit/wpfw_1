const weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.08&longitude=4.31&current=temperature_2m,relative_humidity_2m&timezone=Europe%2FAmsterdam";

// Preserve the public weather feature from the original portfolio.
export async function getWeather(signal) {
  const response = await fetch(weatherUrl, { signal });
  if (!response.ok) throw new Error("Weather could not be loaded. Please try again.");
  const { current } = await response.json();
  if (!Number.isFinite(current?.temperature_2m) || !Number.isFinite(current?.relative_humidity_2m))
    throw new Error("The weather service returned unexpected data.");
  return current;
}
