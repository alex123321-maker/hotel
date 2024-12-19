
var dataTableInstance;
const currentDate = new Date();





$(function () {

    dataTableInstance = $('#customersTable').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/ru.json',
        },
        searching: false,
        scrollY: scrollYcur,
        select: {
            toggleable: true
        },
        columns: dtColumns
    });

    getClients();

    $("#searchInput").on("input", function () {
        var searchQuery = $(this).val().toLowerCase();
        var tableRows = $('#customersTable tbody tr');

        tableRows.each(function () {
            var row = $(this);
            var rowData = row.text().toLowerCase();

            if (rowData.includes(searchQuery)) {
                row.show();
            } else {
                row.hide();
            }
        });
    });

    $('input[type=radio][name=btnradio]').on('change', function () {
        
        let filterStartDate = new Date(); // По умолчанию - сегодня
        switch ($(this).attr('id')) {
            case "btnradio1":
                

                filterReserv(currentDate);
                break;
            case "btnradio2":
                filterStartDate.setDate(currentDate.getDate() - 7)
                filterReserv(filterStartDate);
                break;
            case "btnradio3":
                filterStartDate.setMonth(currentDate.getMonth() - 1);
                filterReserv(filterStartDate);
                break;
            case "btnradio4":
                filterStartDate.setFullYear(currentDate.getFullYear() - 1); 
                filterReserv(filterStartDate);
                break;
            case "btnradio5":
                filterStartDate.setFullYear("2000"); 
                filterReserv(filterStartDate);
                break;
            // Добавьте другие условия по вашему усмотрению
            default:
                break;

        };
        
    });
    $("#myForm").on("submit", function (event) {
        event.preventDefault(); // Отменяем стандартное поведение формы (перезагрузка страницы)

        // Собираем данные из полей формы
        const formData = {
            Name: $("#Name").val(),
            Surname: $("#Surname").val(),
            Lastname: $("#Lastname").val(),
            Passportseries: $("#Passportseries").val(),
            Passportnumber: $("#Passportnumber").val(),
            Email: $("#Email").val(),
            Phone: $("#Phone").val(),
            Prefers: $("#exampleFormControlTextarea1").val()
        };
        
        // Отправка данных на сервер
        $.ajax({
            url: urlCreateClient,
            type: 'POST',
            data: JSON.stringify(formData), // Отправляем данные как JSON
            contentType: 'application/json', // Указываем, что отправляем JSON
            success: function (response) {
                
                // Обработка успешного ответа
                console.log("Запись успешно создана:", response);
                clearFields();
                getClients();
                $("#exampleModalCenter").modal("hide");
                // Дополнительная логика...
                showToast("Клиент успешно добавлен", true);
            },
            error: function (xhr, status, error) {
                
                console.error("Произошла ошибка:", error);
            }
        });
    });
    
});

function getClients() {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            
            var jsonData = JSON.parse(data);
            
            // Очищаем и обновляем данные в таблице
            dataTableInstance.clear().rows.add(jsonData).draw();
            
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function deliteSelected() {
    const selectedRows = dataTableInstance.rows({ selected: true }).data();
    if (selectedRows.length == 0) {
        showToast("Выберите хотя бы одну строку", false)
    }
    else {

        for (let i = 0; i < selectedRows.length; i++) {
            const selectedRow = selectedRows[i];
            const selectedId = selectedRow.Id;

            console.log(selectedRow, selectedId);
            deleteClient(selectedId);
        }
    }

}

function clearFields() {
    const form = document.getElementById("myForm");
    const inputs = form.getElementsByTagName("input");
    const textareas = form.getElementsByTagName("textarea");

    for (let i = 0; i < inputs.length; i++) {
        inputs[i].value = "";
    }

    for (let i = 0; i < textareas.length; i++) {
        textareas[i].value = "";
    }
}

function deleteClient(id) {
    $.ajax({
        url: urlDelite + id,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                getClients(); // Обновляем таблицу после удаления
                showToast("Запись успешно удалена.",true);
            } else {
                console.error("Произошла ошибка при удалении:", response.message);
            }
        },
        error: function (xhr, status, error) {
            showToast(error, false);
            console.error("Произошла ошибка при удалении:", error);
        }
    });
}


function filterReserv(filterStartDate) {
        var tableRows = $('#customersTable tbody tr');
        tableRows.each(function () {
            const row = $(this);
            const hiddenColumnDate = new Date(row.find(".hidden-column").text());

            if (hiddenColumnDate >= filterStartDate) {
                row.show();
            } else {
                row.hide();
            }
        });
    };
