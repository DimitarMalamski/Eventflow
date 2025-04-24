document.addEventListener('DOMContentLoaded', () => {
    const continentSelect = document.getElementById('continentSelect');
    const countrySelect = document.getElementById('countrySelect');

    continentSelect.addEventListener('change', async function () {
        const continentId = this.value;
        if (!continentId) return;

        try {
            const response = await fetch(`/Country/GetCountriesByContinent?continentId=${continentId}`);

            if (!response.ok) throw new Error('Network response was not ok');

            const countries = await response.json();

            countrySelect.innerHTML = '';

            countries.forEach(country => {
                const option = document.createElement('option');
                option.value = country.id;
                option.textContent = country.name;
                countrySelect.appendChild(option);
            });

            // ✅ Add this to re-trigger the change listener on the new dropdown
            countrySelect.dispatchEvent(new Event('change'));
        } catch (error) {
            console.error('Failed to load countries:', error);
            alert('Failed to load countries.');
        }
    });

    // ✅ Trigger calendar update when a country is selected
    countrySelect?.addEventListener('change', function () {
        const countryId = this.value;
        const month = document.getElementById('currentMonth')?.value;
        const year = document.getElementById('currentYear')?.value;

        if (!countryId || !month || !year) {
            console.warn('Missing country, month, or year.');
            return;
        }

        fetch(`/Calendar/LoadCalendarByCountryPartial?countryId=${countryId}&month=${month}&year=${year}`)
            .then(res => res.text())
            .then(html => {
                const wrapper = document.getElementById('calendarOuterWrapper');
                if (wrapper) {
                    wrapper.innerHTML = html;
                    document.dispatchEvent(new Event('calendar:updated'));

                    if (typeof initCalendarUI === 'function') initCalendarUI();
                } else {
                    console.error('calendarOuterWrapper not found.');
                }
            })
            .catch(err => {
                console.error('Error loading national calendar:', err);
            });
    });
});
