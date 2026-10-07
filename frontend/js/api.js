const weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.08&longitude=4.31&current=temperature_2m,relative_humidity_2m&timezone=Europe%2FAmsterdam";

/**
 * Gets JSON data from an API.
 * @param {string} url - The API address.
 * @returns {Promise<Object>} The JSON data. Throws an error if the request fails.
 */
async function fetchJson(url) {
    const response = await fetch(url);

    if (!response.ok) {
        throw new Error("The API request failed.");
    }

    return response.json();
}

/**
 * Gets the current weather in The Hague.
 * @returns {Promise<{temperature_2m: number, relative_humidity_2m: number}>} The temperature and humidity.
 */
async function getWeather() {
    const data = await fetchJson(weatherUrl);
    const weather = data.current;
    const hasWeatherValues = weather
        && Number.isFinite(weather.temperature_2m)
        && Number.isFinite(weather.relative_humidity_2m);

    if (!hasWeatherValues) {
        throw new Error("The API did not return valid weather data.");
    }

    return weather;
}
