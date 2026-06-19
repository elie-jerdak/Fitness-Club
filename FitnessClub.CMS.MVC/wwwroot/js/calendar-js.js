document.addEventListener('DOMContentLoaded', function () {
    const monthYear = document.getElementById('month-year');
    const daysContainer = document.getElementById('days');
    const prevButton = document.getElementById('prev');
    const nextButton = document.getElementById('next');
    const todayButton = document.getElementById('today');
    const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
    ];
    const token = window.authToken;
    let events = {};
    function fetchEvents(callback) {
        fetch('https://localhost:7100/api/calendar/events', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        }) 
            .then(response => response.json())
            .then(data => {
                events = data;
                callback();
            })
            .catch(error => {
                console.error('Failed to load events:', error);
                callback(); // Still render calendar with no events
            });
    }
 
    let currentDate = new Date();

    function renderCalendar(date) {
        const year = date.getFullYear();
        const month = date.getMonth();
        const firstDay = new Date(year, month, 1).getDay();
        const lastDay = new Date(year, month + 1, 0).getDate();
        monthYear.textContent = `${months[month]} ${year}`;
        daysContainer.innerHTML = '';
        // Previous month's dates
        const prevMonthLastDay = new Date(year, month, 0).getDate();
        for (let i = firstDay; i > 0; i--) {
            const dayDiv = document.createElement('div');
            dayDiv.textContent = prevMonthLastDay - i + 1;
            dayDiv.classList.add('fade');
            daysContainer.appendChild(dayDiv);
        }
        // Current month's dates
        for (let i = 1; i <= lastDay; i++) {
            const dayDiv = document.createElement('div');
            dayDiv.textContent = i;
            // Format date for event comparison
            //month + 1 → JavaScript months are 0-indexed (January = 0), so we add 1
            //String(...).padStart(2, '0') → ensures two - digit format(e.g., 07, 01)
            const formattedDate = `${year}-${String(month + 1).padStart(2, '0')}-${String(i).padStart(2, '0')}`;

            // Collect all event keys that start with the current date
            const eventKeys = Object.keys(events).filter(key => key.startsWith(formattedDate));

            if (eventKeys.length > 0) {
                const eventContainer = document.createElement('div');
                eventContainer.classList.add('event-dots');

                const dotsToShow = Math.min(eventKeys.length, 3); // max 3 dots

                for (let j = 0; j < dotsToShow; j++) {
                    const dot = document.createElement('span');
                    dot.classList.add('event-dot');

                    eventContainer.appendChild(dot);
                }

                dayDiv.appendChild(eventContainer);
            }
            if (new Date(year, month, i).toDateString() === new Date().toDateString()) {
                dayDiv.classList.add('today');
            }
            daysContainer.appendChild(dayDiv);
        }
        // Next month's dates
        const nextMonthStartDay = 7 - new Date(year, month + 1, 0).getDay() - 1;
        for (let i = 1; i <= nextMonthStartDay; i++) {
            const dayDiv = document.createElement('div');
            dayDiv.textContent = i;
            dayDiv.classList.add('fade');
            daysContainer.appendChild(dayDiv);
        }
    }

    prevButton.addEventListener('click', function () {
        currentDate.setMonth(currentDate.getMonth() - 1);
        renderCalendar(currentDate);
    });

    nextButton.addEventListener('click', function () {
        currentDate.setMonth(currentDate.getMonth() + 1);
        renderCalendar(currentDate);
    });

    todayButton.addEventListener('click', function () {
        currentDate = new Date();
        renderCalendar(currentDate);
    });

    fetchEvents(() => renderCalendar(currentDate));
});