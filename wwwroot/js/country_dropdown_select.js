document.addEventListener('DOMContentLoaded', () => {
    const continentSelect = document.getElementById('continentSelect');
    const countrySelect = document.getElementById('countrySelect');

    continentSelect.addEventListener('change', async function () {
        const continentId = this.value;
        if (!continentId) {
            return
        }

        try {
            const response = await fetch(`/Country/GetCountriesByContinent?continentId=${continentId}`);

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const countries = await response.json();

            countrySelect.innerHTML = '';

            countries.forEach(country => {
                const option = document.createElement('option');
                option.value = country.id;
                option.textContent = country.name;
                countrySelect.appendChild(option);
            });
        } catch (error) {
            console.error('Failed to load countries:', error);
            alert('Failed to load countries.');
        }
    });
});