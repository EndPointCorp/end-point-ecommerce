// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿import { DataTable } from 'simple-datatables';
import { Chart } from 'chart.js';

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-orders');
    new DataTable(datatablesSimple);

    // Set new default font family and font color to mimic Bootstrap's default styling
    Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
    Chart.defaults.global.defaultFontColor = '#292b2c';

    // Orders chart and data
    var ordersChartLabels = window.dashboardCharts.ordersChartLabels;
    var ordersChartValues = window.dashboardCharts.ordersChartValues;
    var ctx = document.getElementById('ordersChart');
    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
        labels: ordersChartLabels,
        datasets: [{
            label: 'Orders',
            lineTension: 0,
            backgroundColor: 'rgba(2,117,216,0.2)',
            borderColor: 'rgba(2,117,216,1)',
            pointRadius: 5,
            pointBackgroundColor: 'rgba(2,117,216,1)',
            pointBorderColor: 'rgba(255,255,255,0.8)',
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(2,117,216,1)',
            pointHitRadius: 50,
            pointBorderWidth: 2,
            data: ordersChartValues,
        }],
        },
        options: {
        scales: {
            xAxes: [{
            time: {
                unit: 'date'
            },
            gridLines: {
                display: false
            },
            ticks: {
                maxTicksLimit: 7
            }
            }],
            yAxes: [{
            ticks: {
                min: 0,
                maxTicksLimit: 5
            },
            gridLines: {
                color: 'rgba(0, 0, 0, .125)',
            }
            }],
        },
        legend: {
            display: false
        }
        }
    });

    // Orders chart and data
    var amountsChartLabels = window.dashboardCharts.amountsChartLabels;
    var amountsChartValues = window.dashboardCharts.amountsChartValues;
    var ctx = document.getElementById('amountsChart');
    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
        labels: amountsChartLabels,
        datasets: [{
            label: 'Amount',
            lineTension: 0,
            backgroundColor: 'rgba(2,117,216,0.2)',
            borderColor: 'rgba(2,117,216,1)',
            pointRadius: 5,
            pointBackgroundColor: 'rgba(2,117,216,1)',
            pointBorderColor: 'rgba(255,255,255,0.8)',
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(2,117,216,1)',
            pointHitRadius: 50,
            pointBorderWidth: 2,
            data: amountsChartValues,
        }],
        },
        options: {
        scales: {
            xAxes: [{
            time: {
                unit: 'date'
            },
            gridLines: {
                display: false
            },
            ticks: {
                maxTicksLimit: 7
            }
            }],
            yAxes: [{
            ticks: {
                min: 0,
                maxTicksLimit: 5
            },
            gridLines: {
                color: 'rgba(0, 0, 0, .125)',
            }
            }],
        },
        legend: {
            display: false
        }
        }
    });
});
