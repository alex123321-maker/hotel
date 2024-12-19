var cDate = new Date();
var table = null; // Объявляем переменную для хранения экземпляра DataTable
var resprice = $("#resPrice")

var roomPrices = {}

const dayOfWeekMapping = {
    0:"Воскресенье",
    1:"Понедельник",
    2:"Вторник",
    3:"Среда",
    4:"Четверг",
    5:"Пятница",
    6:"Суббота",
};
function parseDate(dateString) {
    const parts = dateString.split(".");
    var year;
    if (parts.length < 3) {
        year = new Date().getFullYear();
    } else {
        year = parseInt(parts[2]);
    }
    const day = parseInt(parts[0]);
    const month = parseInt(parts[1]) - 1; // Месяцы в JavaScript идут с 0 до 11
    return new Date(year, month, day);
}
function getDayOfWeekIndex(dayName) {
    return dayOfWeekMapping[dayName] ?? null; // Если день не найден, вернётся null
}

function calculateBookingCost(checkInDate, checkOutDate, dailyRate, pricing, discount) {
    const checkIn = parseDate(checkInDate);
    const checkOut = parseDate(checkOutDate);
    


    

    const differenceInMilliseconds = checkOut - checkIn;
    const differenceInDays = Math.ceil(differenceInMilliseconds / (24 * 60 * 60 * 1000));

    

    if (differenceInDays <= 0) {
        throw new Error("Некорректный диапазон дат");
    }
    var dis = 0;
    discount.forEach(el => {

        if (el.Days <= differenceInDays) { dis = el.Amount } ;

    })

    let totalCost = 0;

    for (let i = 0; i < differenceInDays; i++) {
        const currentDay = new Date(checkIn);
        currentDay.setDate(currentDay.getDate() + i);

        const dayOfWeekIndex = currentDay.getDay();

        const multiplier = pricing.find(el => el.day_of_week === dayOfWeekMapping[dayOfWeekIndex])?.multiplier || 1;

        
        totalCost += dailyRate * multiplier;
    }

    
    totalCost *= (100 - dis)/100;

    return totalCost.toFixed(2);
}

function updateReservationPrice() {


    const checkInDate = $("#checkInDateinput").val(); // Дата заезда
    const checkOutDate = $("#checkOutDateinput").val(); // Дата выезда
    const roomId = $("#roomInput").val(); // Номер комнаты

    if (!checkInDate || !checkOutDate || !roomId) {
        $("#resPrice").text("0"); // Если данные неполные, устанавливаем цену в 0
        return;
    }

    // Базовая стоимость номера по ID
    const dailyRate = rooms.find(room => room.Id === parseInt(roomId))?.Price || 0;

    if (dailyRate === 0) {
        $("#resPrice").text("0");
        return;
    }

    try {
        // Вызываем функцию расчёта стоимости
        const totalPrice = calculateBookingCost(
            checkInDate,
            checkOutDate,
            dailyRate,
            pricing,
            discounts
        );

        // Обновляем стоимость на странице
        $("#resPrice").text(totalPrice + " р");
    } catch (error) {
        console.error("Ошибка при расчёте стоимости: ", error.message);
        $("#resPrice").text("Ошибка");
    }
}
$(document).ready(function () {
   
    // События на изменение значений
    

    // Также вызываем пересчёт стоимости при загрузке модального окна
    $("#exampleModalCenter").on("shown.bs.modal", updateReservationPrice);
});

$(function () {
    
    

    $("#reservForm").on("submit", function (event) {
        event.preventDefault(); // Отменяем стандартное поведение формы (перезагрузка страницы)

        // Собираем данные из полей формы
        const formData = formdata
        formData.CustomerId = document.getElementById("clientInput").getAttribute("data");
        formData.RoomId = document.getElementById("roomInput").value;
        formData.CheckInDate = document.getElementById("checkInDateinput").value;
        formData.CheckOutDate = document.getElementById("checkOutDateinput").value;

        try {
            discounts.forEach(el => {
                differenceInMilliseconds = parseDate(formData.CheckOutDate) - parseDate(formData.CheckInDate);
                var differenceInDays = Math.floor(differenceInMilliseconds / (24 * 60 * 60 * 1000));
                
                if (differenceInDays >= el.Days) {
                    formData.DiscountId = el.Id;
                    throw new Error("Выход из цикла");
                }
            });
        } catch (e) {
            if (e.message !== "Выход из цикла") {
                throw e; // Если это не наше исключение, перебрасываем его дальше
            }
        }

        
         
            $.ajax({
                url: urlCreateClient,
                type: 'POST',
                data: JSON.stringify(formData), // Отправляем данные как JSON
                contentType: 'application/json', // Указываем, что отправляем JSON
                success: function (response) {
                    // Обработка успешного ответа
                    if (response.success) {
                        getClients();
                        drawTable(cDate);
                        $("#exampleModalCenter").modal("hide");
                        // Дополнительная логика...
                        showToast("Бронь успешно добавлена", true);
                    }
                    else {
                        showToast("Ошибкуа при добавлении: "+ response.message, false);
                    }
                },
                error: function (xhr, status, error) {
                    // Обработка ошибки
                    showToast(error, false);
                }
            });
    });

    
    function getDates(curdate) {
        var dates = [];

        for (var i = 0; i <= 20 ; i++) {
            var nextDate = new Date(curdate);
            nextDate.setDate(curdate.getDate() + i);
            dates.push(nextDate);
        }
        return dates;
    }

    function FillTable(dates) {

        var table = document.getElementById("roomsTable");
        var thead = table.querySelector("thead");
        var tr1 = thead.querySelector("tr");
        tr1.replaceChildren();

        var paras = document.getElementsByClassName('created');

        while (paras[0]) {
            paras[0].parentNode.removeChild(paras[0]);
        }

        var typeRoomTh = document.createElement("th");
        typeRoomTh.setAttribute("rowspan", "2");
        typeRoomTh.textContent = "Тип";
        tr1.appendChild(typeRoomTh);

        var capacityTh = document.createElement("th");
        capacityTh.setAttribute("rowspan", "2");
        capacityTh.textContent = "Вместимость";
        tr1.appendChild(capacityTh);

        var roomTh = document.createElement("th");
        roomTh.setAttribute("rowspan", "2");
        roomTh.textContent = "Номер";
        tr1.appendChild(roomTh);
        var columns = [{ data: "Тип", class: "no-select" }, { data: "Вместимость", class: "no-select"}, { data: "Номер", class: "no-select"}];
        var tr2 = document.createElement("tr");
        dates.forEach((date) => {
            var dateParts = date.toLocaleDateString('en-En', { month: '2-digit', day: '2-digit' }).replace(/\./g, '-').split('/');
            var th = document.createElement("th");
            th.classList.add('dateCells');

            var monthExists = false;

            tr1.querySelectorAll("th").forEach((element) => {
                if (element.textContent === dateParts[0]) {
                    monthExists = true;
                    element.setAttribute("colspan", parseInt(element.getAttribute("colspan")) + 1);
                    return; // Если элемент найден, можно выйти из цикла
                }
            });

            if (!monthExists) {
                var thm = document.createElement("th");
                thm.textContent = dateParts[0];
                thm.classList.add('month');
                thm.setAttribute("colspan", "1"); // Объединение ячеек для месяца
                tr1.appendChild(thm);
            }

            var dayDiv = document.createElement("div");
            dayDiv.classList.add('dateDay');
            dayDiv.textContent = dateParts[1]; // День (допустим, что dateParts[2] содержит день)
            th.setAttribute("date", date.toLocaleDateString())
            th.appendChild(dayDiv);

            columns.push({
                data: date,
                orderable: false,
                createdCell: function (td, cellData, rowData, row, col) {
                    // Добавляем класс в зависимости от значения ячейки
                    if (cellData === '1') {
                        td.classList.add('no-select'); // Класс для ячейки с 1

                        td.classList.add('cell-red'); // Класс для ячейки с 1
                    } else if (cellData === '0') {
                        td.classList.add('cell-green'); // Класс для ячейки с 0
                    }
                }
            });
            tr2.appendChild(th);
        });
        tr2.classList.add("created");
        thead.appendChild(tr2);

        return columns;
    }

    function shiftWeekBackward() {
        cDate.setDate(cDate.getDate() - 7);

        drawTable(cDate); // Обновление таблицы с новой датой
    }

    function shiftWeekForward() {
        cDate.setDate(cDate.getDate() + 7);
        drawTable(cDate); // Обновление таблицы с новой датой
    }

    $("#roomTypeFilter").change(function () {
        var selectedRoomType = $(this).val(); 
        var selectedCaps = $("#roomCapsFilter").val();
        table.rows().every(function () {
            var rowData = this.data(); 
            var caps = rowData.Вместимость;
            var ColumnText = rowData.Тип;
            if ((caps == selectedCaps || selectedCaps == "Вместимость комнат" ) & (ColumnText == selectedRoomType || selectedRoomType == "Тип комнаты")) {
                this.nodes().to$().show();
            }
            else {
                this.nodes().to$().hide();
            }
        })


    });
    $("#roomCapsFilter").change(function () {
        var selectedCaps = $(this).val(); 
        var selectedRoomType = $("#roomTypeFilter").val();
        table.rows().every(function () {
            var rowData = this.data();
            var ColumnText = rowData.Вместимость;
            var roomtype = rowData.Тип;
            if ((roomtype == selectedRoomType || selectedRoomType == "Тип комнаты") & (ColumnText == selectedCaps || selectedCaps =="Вместимость комнат")) {
                this.nodes().to$().show();
            }
            else {
                this.nodes().to$().hide();
            }
        })


    });

    function drawTable(fromdate) {

        $("#checkInDateinput").val("-").trigger("input");
        $("#checkOutDateinput").val("-").trigger("input");
        $("#roomInput").val("-").trigger("input")
        if (table) {
            table.destroy();
            

        }
        var dates = getDates(fromdate);
        var cols = FillTable(dates);

        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var rawdata = JSON.parse(data);
                var data = {};
                rawdata.forEach((item) => {
                    
                    var roomNumber = item.Room.Id;
                    roomPrices[roomNumber] = item.Room.Price

                    if (!data[roomNumber]) {
                        data[roomNumber] = {
                            "Номер": item.Room.Id,
                            "Тип": item.Room.Type,
                            "Вместимость": item.Room.Capacity
                        };
                        dates.forEach((date) => {
                            data[roomNumber][date] = "0"; // Устанавливаем значение по умолчанию
                        });
                    }

                    var ChkInDt =new Date(item.CheckInDate);
                    var ChkOutDt =new Date(item.CheckOutDate);
                    dates.forEach((date) => {
                        var dateObj = date;
                        var curDate = new Date();
                        if ((ChkInDt <= dateObj && dateObj < ChkOutDt)) {
                            data[roomNumber][date] = "1"; // Устанавливаем значение "1" для занятых дат
                        }
                    });
                });
                rooms.forEach(el => {
                    if (!(el.Id in data)) {

                        data[el.Id] = {
                            "Номер": el.Id,
                            "Тип": el.RoomType.Type,
                            "Вместимость": el.Capacity.Capacity
                        };
                        dates.forEach(date => {
                            

                            data[el.Id][date] = "0";
                            
                        });
                        
                    }
                })

                var tableData = Object.values(data);
                table.rows.add(tableData).draw();
                
                
                
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });


        table = new DataTable('#roomsTable', {
            data: [],
            paging: false,
            deferRender: true,
            searching: false,
            columns: cols,
            scrollY: scrollYrooms,
            select: {
                style: 'multi',
                selector: 'td:not(.no-select)',
                items: 'cell'
            },
            dom: 'Bfrtip', // Добавляем кнопки в DOM
            buttons: [
                {
                    text: 'Неделя назад',
                    className: 'btn btn-primary clicked',
                    action: function () {
                        shiftWeekBackward();
                    }
                },
                {
                    text: 'Неделя вперёд',
                    className: 'btn btn-primary clicked',
                    action: function () {
                        shiftWeekForward();
                    }
                }
            ]
        });
        table.select.style('api');
        table.off('click', 'td');
        table.on('click', 'td', function (e) {
            var curCell = table.cell(e.target);
            if (curCell.selected()) {
                curCell.deselect();
                var cols = [];
                var cols1 = [];
                var selectedCells = table.cells({ selected: true });
                selectedCells.every(function () { if (this.index().column < curCell.index().column) { cols.push(this.index()); } else { cols1.push(this.index()); }; });
                if ((selectedCells.count() / 2) < (selectedCells.count() - cols.length)) {
                    cols.forEach((el) => { table.cell(el.row, el.column).deselect(); });

                }
                else {
                    cols1.forEach((el) => { table.cell(el.row, el.column).deselect(); });

                }
                var cells = table.cells({ selected: true }).toArray();
                if (cells[0].length != 0) {
                    $("#checkInDateinput").val(table.column(cells[0][0].column).header().getAttribute("date")).trigger("input");
                    var OutDate = parseDate(table.column(cells[0][cells[0].length - 1].column).header().getAttribute("date"));
                    OutDate.setDate(OutDate.getDate() + 1)


                    $("#checkOutDateinput").val(OutDate.toLocaleDateString());
                } else {
                    $("#checkInDateinput").val("-");
                    $("#checkOutDateinput").val("-");
                    $("#roomInput").val("-").trigger("input");
                };
                updateReservationPrice()

                return

            }
            else {
                if (curCell.node().classList.contains('no-select')) {


                    return;
                }
                else {
                    curCell.select();

                }
            }
            var rows = new Set();
            var selectedCells = table.cells({ selected: true });
            table.cells({ selected: true }).every(function () { rows.add(this.index().row); });
            if ((selectedCells.count() >= 2) && (rows.size == 1)) {
                columns = [];
                var row;
                selectedCells.toArray().forEach((list) => { list.forEach((obj) => { columns.push(obj.column); row = obj.row; }); });

                var leftBorder;
                var rightBorder;

                if (curCell.index().column > columns[0]) {
                    leftBorder = columns[0];
                    rightBorder = curCell.index().column;
                } else {
                    leftBorder = curCell.index().column;
                    rightBorder = columns[columns.length - 1];
                }

                // Clear previous selection

                selectedCells.deselect();
                // Select all cells between the first and last selected cells
                for (var column = leftBorder; column <= rightBorder; column++) {
                    var cell = table.cell(row, column);
                    if (cell.node().classList.contains('no-select')) {
                        table.cells({ selected: true }).deselect();
                        $("#checkInDateinput").val("-");
                        $("#checkOutDateinput").val("-");
                        $("#roomInput").val("-").trigger("input");
                        return;
                    }
                    else {
                        table.cell(row, column).select();
                    }

                }
                updateReservationPrice()

            }
            var cells = table.cells({ selected: true }).toArray();
            if (cells[0].length != 0) {
                $("#checkInDateinput").val(table.column(cells[0][0].column).header().getAttribute("date"));
                var OutDate = parseDate(table.column(cells[0][cells[0].length - 1].column).header().getAttribute("date"));
                OutDate.setDate(OutDate.getDate() + 1)
                $("#checkOutDateinput").val(OutDate.toLocaleDateString()).trigger("input");
            } else {
                $("#checkInDateinput").val("-");
                $("#checkOutDateinput").val("-");
                $("#roomInput").val("-").trigger("input");

            };
            updateReservationPrice()


        });
        table.off('select');
        table.on('select', function (e, dt, type, indexes) {
            var rows = new Set();
            table.cells({ selected: true }).every(function () { rows.add(this.index().row); });
            if (rows.size > 1) {
                table.cells({ selected: true }).deselect();
                table.cell(indexes[0].row, indexes[0].column).select();
            }
            $("#roomInput").val((table.cell(indexes[0].row, 2).data())).trigger("input");
            updateReservationPrice()
        });

    }
    $("#exampleModalCenter").on('shown.bs.modal', () => {
        table.columns.adjust().draw();

    })
    drawTable(cDate);



    
   
    
});
