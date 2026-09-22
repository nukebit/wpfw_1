# Portfolio Brian Arjun — WPFW opdracht 1 en 2

Dit is een portfoliosite met drie pagina's: een introductie, projecten en een blog. Opdracht 1 leverde de HTML- en CSS-basis. Opdracht 2 voegt JavaScript-interactie toe. De site heeft geen backend of database.

## Bekijken

Open `index.html` in een browser. De projectpagina staat onder `pages/projects.html`. Voor het weerblok is een internetverbinding nodig.

## Wat is toegevoegd voor opdracht 2?

- `js/script.js` bevat de projectgegevens als array. `renderProjects()` filtert en sorteert de array en maakt de kaarten met `createElement()`, `textContent`, `append()` en `replaceChildren()`. De keuzelijsten reageren op `change`.
- In hetzelfde bestand controleren `getErrorMessage()` en `validateField()` naam, e-mailadres en bericht wanneer het formulier wordt ingediend (`submit`). Bij nieuwe invoer worden oude meldingen gewist (`input`). Voor het e-mailadres gebruiken ze de ingebouwde controle van het `type="email"`-veld. Fouten zijn aan de velden gekoppeld met `aria-describedby` en `aria-invalid`; de status wordt aangekondigd met `aria-live`.
- `loadWeather()` haalt met `fetch()` de actuele temperatuur en luchtvochtigheid voor Den Haag op bij [Open-Meteo](https://open-meteo.com/en/docs). `showWeather()` maakt de inhoud met DOM-methoden. Bij een fout is er een knop om opnieuw te proberen.

Elk project heeft precies vier velden: `type`, `title`, `image` en `description`. De filter vergelijkt `type` met de gekozen optie. De link-id en alternatieve afbeeldingstekst worden afgeleid van de titel. Het weerblok leest `current.temperature_2m` en `current.relative_humidity_2m` uit de JSON-respons. Er is geen API-sleutel nodig.

Het contactformulier is een client-side demonstratie. Na geldige invoer verschijnt een bevestiging dat de velden kloppen; er wordt geen bericht verzonden.

## Zelf controleren

1. Open de projectpagina: er zijn drie projecten. Kies **Applicaties** en **Dashboards**, en probeer daarna **Naam A-Z**.
2. Verstuur het lege contactformulier: elk veld krijgt een eigen foutmelding. Vul daarna een ongeldig e-mailadres en een bericht van minder dan 10 tekens in.
3. Vul alle velden geldig in: er verschijnt een bevestiging zonder dat het formulier de pagina verlaat.
4. Open de startpagina met internet: het weerblok toont temperatuur en luchtvochtigheid. Zonder verbinding verschijnt een foutmelding met **Try again**.

## Procesverantwoording voor Boulder

Vul deze gegevens zelf aan met je echte proces voordat je inlevert:

- **Studentnummer, klas, beoordelaar en datum:** nog invullen.
- **Aanpak en eigen keuzes:** beschrijf hoe je de code hebt bekeken, aangepast en gecontroleerd.
- **Bronnen:** [Open-Meteo documentatie](https://open-meteo.com/en/docs) en eventuele andere bronnen die je zelf hebt gebruikt.
- **AI-gebruik:** rapporteer eerlijk dat Codex op verzoek de implementatie voor opdracht 2 heeft gemaakt; beschrijf welke delen je zelf hebt gecontroleerd en gewijzigd. De opdrachttekst noemt AIAS-niveau 2, dus bespreek met je docent of en hoe je dit werk mag gebruiken als bewijsstuk.
- **Ontvangen en verwerkte feedback:** nog invullen na een feedbackmoment.
- **Zelfstudie-uren:** noteer je werkelijk bestede uren.

Inleveren via Brightspace en Boulder moet je zelf doen. Deze repository bevat de site en de technische toelichting; er is niets gepusht.
