var ctx = document.getElementById("MyChart");
var myPieChart = new Chart(ctx,{
    type: 'pie',
    data: {
		datasets: [{
			label: 'Points',
			backgroundColor: ['#f1c40f','#e67e22','#16a085'],
			data: [10, 20, 30]
		}],
		labels: [
			'Red',
			'Yellow',
			'Blue'
		]
	},
	options: {
		responsive: false
	}
});