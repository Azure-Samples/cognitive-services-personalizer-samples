document.addEventListener("DOMContentLoaded", function () {

    let labels = [];
    const maxLoop = 1000;
    const hoops = 5;
    for (var i = 1; i <= maxLoop / hoops; i++) {
        labels.push(i * hoops);
    }

    let data = [];
    let currentValue = 0;
    const maxValue = 0.8;
    for (i = 1; i <= maxLoop / hoops; i++) {
        let max = currentValue + 0.03;
        let min = currentValue - 0.01;
        
        currentValue = Math.random() * (max - min) + min;
        if (currentValue > maxValue) {
            currentValue = maxValue;
        }

        data.push(currentValue);
    }

    const ctx = document.getElementById('avg-learn-chart');
    const avgLearnChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                fillColor: "rgba(172,194,132,0.4)",
                strokeColor: "#ACC26D",
                pointColor: "#fff",
                pointStrokeColor: "#9DB86D",
                data: []
            }]
        },
        options: {
            maintainAspectRatio: false,
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        min: 0,
                        max: 1,
                        stepSize: 0.1,
                        suggestedMin: 0,
                        suggestedMax: 1
                    }
                }]
            }
        }
    });

    function addData(chart, data) {
        chart.data.datasets.forEach((dataset) => {
            dataset.data.push(data);
        });
        chart.update();
    }

    const maxTick = maxLoop / hoops;

    let currentTick = 0;
    let intervalId = setInterval(function () {
        if (currentTick >= maxTick) {
            clearInterval(intervalId);
        }

        addData(avgLearnChart, data[currentTick]);
        currentTick++;

    }, 70);
});