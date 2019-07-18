﻿document.addEventListener("DOMContentLoaded", function () {

    let labels = [];
    const maxLoop = 1000;
    const hoops = 5;
    for (var i = 1; i <= maxLoop / hoops; i++) {
        labels.push(i * hoops);
    }

    let data = [];
    let dataWithout = [];
    let currentValue = 0.05;
    let decreasedValue = 0;
    const maxValue = 0.8;
    const maxWithoutValue = 0.6;

    function getRandomValue(currentValue, maxDelta, minDelta, withoutPersonalizer) {
        let max = currentValue + maxDelta;
        let min = currentValue - minDelta;
        let newValue = Math.random() * (max - min) + min;

        if (!withoutPersonalizer) {
            if (newValue < currentValue) {
                return currentValue;
            }
        } else {
            if (newValue > maxWithoutValue) {
                return maxWithoutValue;
            }
        }
        
        return newValue - (Math.random() > 0.5 ? Math.random() * 0.02 : Math.random() * 0.02 * -1);
    }

    function getFinalValue(currentValue, withoutPersonalizer) {
        //let finalValue = (Math.log(currentValue + 0.37) + 1)/1.5;
        let finalValue;
        if (withoutPersonalizer) {
            finalValue = currentValue;
        } else {
            finalValue = (Math.log(currentValue + 0.15) + 2) / 2.3;
            if (finalValue > maxValue) {
                return maxValue - (Math.random() > 0.5 ? Math.random() * 0.02 : Math.random() * 0.02 * -1);
            }
        }

        return finalValue;
    }

    for (i = 1; i <= maxLoop / hoops; i++) {
        currentValue = getRandomValue(currentValue, 0.02, 0.01, false);
        decreasedValue = getRandomValue(0.25, 0.05, 0, true);

        data.push(getFinalValue(currentValue, false));
        dataWithout.push(getFinalValue(decreasedValue,true));
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
                label: "A",
                backgroundColor: "rgba(0,153,0,0.2)",
                borderColor: "rgba(0,153,0,1)",
                pointColor: "#fff",
                pointStrokeColor: "#9DB86D",
                data: []
            },
                {
                    label: "B",
                    borderColor: "rgba(200,200,200,1)",
                    backgroundColor: "rgba(0,0,0,0)",
                    data: []
                }
            ]
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
                    },
                    gridLines: {
                        display: false
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
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(54, 162, 235, 0.2)'
                ],
                borderColor: [
                    'rgba(54, 162, 235, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(54, 162, 235, 1)',
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
                    },
                    gridLines: {
                        display: false
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

    function updateData(avgLearnChart, peopleChart,  data, dataWithout, currentTick) {
        avgLearnChart.data.datasets[0].data.push(data);
        avgLearnChart.data.datasets[1].data.push(dataWithout);
        avgLearnChart.update();
               
        peopleChart.data.datasets[0].data = [
            getRandomValue(data, 0.05, 0.05, false),
            getRandomValue(data, 0.05, 0.05, false),
            getRandomValue(data, 0.05, 0.05, false),
            getRandomValue(data, 0.05, 0.05, false)
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
        avgLearnChart.data.datasets[1].data = [];
        avgLearnChart.update();

        let currentTick = 0;
        intervalId = setInterval(function () {
            if (currentTick >= maxTick) {
                clearInterval(intervalId);
                return;
            }

            updateData(avgLearnChart, peopleChart, data[currentTick], dataWithout[currentTick], currentTick);
            currentTick++;

        }, 70);
    });
});