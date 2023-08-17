var dataTableInstance;

$(document).ready(function () {

    dataTableInstance = $('#customersTable').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/ru.json',
        },
        searching: false,
        scrollY: 600,
        select: {
            toggleable: true
        },
        columns: [
            { data: 'Id'},
            { data: 'Name' },
            { data: 'Surname' },
            { data: 'Lastname' },
            { data: 'Email' },
            { data: 'Phone' },
            { data: 'Passportseries' },
            { data: 'Passportnumber' },
            { data: 'Prefers' }
        ]
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
                // Обработка ошибки
                showToast(error, false);
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
        url: '/Home/DeleteClient/' + id,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                console.log("Запись успешно удалена:", response.message);
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

function showToast(message, success) {
    const toastPlacement = document.getElementById("toastPlacement");
    const toast = toastPlacement.querySelector(".toast");

    // Очищаем классы перед добавлением новых
    toast.classList.remove("bg-success", "bg-danger", "text-white");

    const toastheader = toast.querySelector(".toast-header");
    const toastBody = toast.querySelector(".toast-body");

    // Очищаем содержимое заголовка перед добавлением нового
    toastheader.innerHTML = "";

    if (success) {
        toast.classList.add("bg-success", "text-white");

        var strongElement = document.createElement("strong");
        strongElement.className = "me-auto";
        strongElement.textContent = "Успешно";

        toastheader.appendChild(strongElement);
    } else {
        toast.classList.add("bg-danger", "text-white");

        var strongElement = document.createElement("strong");
        strongElement.className = "me-auto";
        strongElement.textContent = "Ошибка";

        toastheader.appendChild(strongElement);
    }

    // Устанавливаем текст в теле тоста
    toastBody.textContent = message;

    const bsToast = new bootstrap.Toast(toast); // Инициализируем тост
    bsToast.show(); // Показываем тост
}

