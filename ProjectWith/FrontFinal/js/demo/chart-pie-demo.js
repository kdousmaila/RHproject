// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#8587//96';




/* Pie Chart Example*/
//var ctx = document.getElementById("myPieChart");
//var myPieChart = new Chart(ctx, {
//  type: 'doughnut',
//  data: {
//      labels: ["valide", "rejete", "sans reponse"],
//    datasets: [{
//      data: [80, 10, 10],
//      backgroundColor: ['#a81831', '#1cc88a', '#36b9cc'],
//      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
//      hoverBorderColor: "rgba(234, 236, 244, 1)",
//    }],
//  },
//  options: {
//    maintainAspectRatio: false,
//    tooltips: {
//      backgroundColor: "rgb(255,255,255)",
//      bodyFontColor: "#858796",
//      borderColor: '#dddfeb',
//      borderWidth: 1,
//      xPadding: 15,
//      yPadding: 15,
//      displayColors: false,
//      caretPadding: 10,
//    },
//    legend: {
//      display: false
//    },
//    cutoutPercentage: 80,
//  },
//});


$(document).ready(function () {
    $.ajax({
        url: '/Home/GetDataJSON', // Assurez-vous d'utiliser le bon chemin vers votre action dans le contrôleur
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var ctx = document.getElementById("myPieChart").getContext("2d");

            var myPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ["Demande valid\u00e9e", "Demande rejet\u00e9e", "Demande sans r\u00e9ponse"],
                    datasets: [{
                        data: [
                            data.ClotData,       // Utilisez les données retournées par l'appel AJAX
                            data.RetardData,     // Utilisez les données retournées par l'appel AJAX
                            data.NonClotData     // Utilisez les données retournées par l'appel AJAX
                        ],
                        backgroundColor: ['#a81831', '#1cc88a', '#36b9cc'],
                        hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: false
                    },
                    cutoutPercentage: 80,
                },
            });
        }
    });
});




