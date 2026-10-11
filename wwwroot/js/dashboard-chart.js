window.renderMixedChart = (dotNetRef, canvasId, labels, attemptsData, scoreData) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    // Hủy biểu đồ cũ nếu có
    const existingChart = Chart.getChart(canvasId);
    if (existingChart) {
        existingChart.destroy();
    }

    // Tạo màu gradient
    const gradientBar = ctx.createLinearGradient(0, 0, 0, 400);
    gradientBar.addColorStop(0, 'rgba(54, 162, 235, 0.8)');
    gradientBar.addColorStop(1, 'rgba(54, 162, 235, 0.1)');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    // CỘT: Số lượt thi
                    label: 'Lượt thi',
                    data: attemptsData,
                    backgroundColor: gradientBar,
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1,
                    borderRadius: 4,
                    order: 2,
                    yAxisID: 'y',
                },
                {
                    // ĐƯỜNG: Điểm trung bình
                    type: 'line',
                    label: 'Điểm TB',
                    data: scoreData,
                    borderColor: '#ff6384',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderWidth: 3,
                    tension: 0.4,
                    pointBackgroundColor: '#fff',
                    pointBorderColor: '#ff6384',
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    fill: false,
                    order: 1,
                    yAxisID: 'y1',
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            // [FIX GUI] Đổi con trỏ chuột thành bàn tay khi hover vào cột
            onHover: (event, chartElement) => {
                event.native.target.style.cursor = chartElement[0] ? 'pointer' : 'default';
            },
            onClick: (e, elements) => {
                if (elements && elements.length > 0) {
                    const index = elements[0].index;
                    // Gọi về C#
                    dotNetRef.invokeMethodAsync('HandleChartClick', index)
                        .catch(err => console.log("Click ignored:", err));
                }
            },
            interaction: {
                mode: 'index',
                intersect: false,
            },
            plugins: {
                legend: { position: 'top' },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 10,
                    cornerRadius: 4
                }
            },
            scales: {
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    beginAtZero: true,
                    title: { display: true, text: 'Lượt thi' },
                    grid: { color: '#f0f0f0' }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    min: 0,
                    max: 10,
                    grid: { display: false },
                    title: { display: true, text: 'Điểm số' }
                },
                x: {
                    grid: { display: false }
                }
            }
        }
    });
};