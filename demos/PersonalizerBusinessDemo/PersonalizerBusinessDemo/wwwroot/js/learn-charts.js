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

    function getRandomValue(currentValue, maxDelta, minDelta) {
        let max = currentValue + maxDelta;
        let min = currentValue - minDelta;

        let newValue = Math.random() * (max - min) + min;
        if (newValue < currentValue) {
            return currentValue;
        }
        if (newValue > maxValue) {
            return maxValue;
        }
        return newValue;
    }

    function getFinalValue(currentValue) {
        //let finalValue = (Math.log(currentValue + 0.37) + 1)/1.5;
        let finalValue = (Math.log(currentValue + 0.15) + 2) /2.3;

        if (finalValue > maxValue) {
            return maxValue;
        }

        return finalValue;
    }

    for (i = 1; i <= maxLoop / hoops; i++) {
        currentValue = getRandomValue(currentValue, 0.02, 0.01);
        console.log("currentValue", getFinalValue(currentValue));
        data.push(getFinalValue(currentValue));
    }

    const startLearnBtnEle = document.getElementById("start-learn-btn");
    const currentAvgNumberEle = document.getElementById('current-avg-number');
    const currentAvgNumberLbl = document.getElementById('current-avg-number-label');

    const avgCtx = document.getElementById('avg-learn-chart');
    const avgLearnChart = new Chart(avgCtx, {
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
            title: {
                display: true,
                text: ''
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
                    },
                    scaleLabel: {
                        display: true,
                        labelString: 'User engagement (Reward)'
                    }
                }],
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Home Page Visits'
                    }
                }]
            }
        }
    });


    const peopleCtx = document.getElementById('people-learn-chart');
    const peopleChart = new Chart(peopleCtx, {
        type: 'bar',
        data: {
            labels: ["Mobile", "Social Media", "Anonymous Users"],
            datasets: [{
                data: [0, 0, 0],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            maintainAspectRatio: false,
            legend: {
                display: false
            },
            title: {
                display: true,
                text: 'Engagement by user segment'
            },
            scales: {
                xAxes: [{
                    ticks: {
                        autoSkip: false,
                        maxRotation: 70,
                        minRotation: 0
                    }
                }],
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

    function updateData(avgLearnChart, peopleChart,  data, currentTick) {
        avgLearnChart.data.datasets[0].data.push(data);
        avgLearnChart.update();
               
        peopleChart.data.datasets[0].data = [
            getRandomValue(data, 0.05, 0.05),
            getRandomValue(data, 0.05, 0.05),
            getRandomValue(data, 0.05, 0.05),
            getRandomValue(data, 0.05, 0.05)
        ];
        peopleChart.update();

        currentAvgNumberEle.innerHTML = parseFloat(Math.round(data * 100) / 100).toFixed(2);
        currentAvgNumberLbl.innerHTML = 'Average Engagement last 100 visits';
    }

    const maxTick = maxLoop / hoops;
    let intervalId = -1;

    startLearnBtnEle.addEventListener("click", function () {
        if (intervalId >= 0) {
            clearInterval(intervalId);
        }

        avgLearnChart.data.datasets[0].data = [];
        avgLearnChart.update();

        let currentTick = 0;
        intervalId = setInterval(function () {
            if (currentTick >= maxTick) {
                clearInterval(intervalId);
                return;
            }

            updateData(avgLearnChart, peopleChart, data[currentTick], currentTick);
            currentTick++;

        }, 70);
    });
});