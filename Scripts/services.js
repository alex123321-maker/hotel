$(document).ready(function () {
    let currentReservationId = null;

    // Делегирование событий для динамически добавленных кнопок
    $(document).on('click', '.manage-services', function () {
        const reservationId = $(this).data('id');
        if (!reservationId) {
            console.error("Reservation ID is undefined");
            showToast("Ошибка: ID бронирования отсутствует", false);
            return;
        }

        console.log("Открытие модального окна для бронирования ID:", reservationId);

        currentReservationId = reservationId;
        loadReservationServices(currentReservationId);
        loadAvailableServices();
    });

    // Загрузка услуг для текущего бронирования
    // Загрузка услуг для текущего бронирования
    function loadReservationServices(reservationId) {
        $.get(`/Home/GetReservationServices?reservationId=${reservationId}`)
            .done(function (services) {
                const servicesList = $('#servicesList');
                servicesList.empty();

                if (services.length === 0) {
                    servicesList.append(`<tr><td colspan="4" class="text-center">Услуг нет</td></tr>`);
                } else {
                    services.forEach(service => {
                        console.log(service.Complited)
                        const isCompleted = service.Complited ? "Выполнена" : "Не выполнена";
                        servicesList.append(`
                        <tr data-id="${service.Id}">
                            <td>${service.Title}</td>
                            <td>${service.Price} ₽</td>
                            <td>${isCompleted}</td>
                            <td>
                                <button class="btn btn-success toggle-completion" data-id="${service.Id}" data-completed="${service.Complited}">
                                    ${service.Complited ? "Снять выполнение" : "Выполнить"}
                                </button>
                                <button class="btn btn-danger remove-service" data-id="${service.Id}">
                                    Удалить
                                </button>
                            </td>
                        </tr>
                    `);
                    });
                }

                // Подключение обработчиков событий для кнопок
                $('.toggle-completion').off('click').on('click', function () {
                    const serviceId = $(this).data('id');
                    $.post('/Home/ToggleServiceCompletion', { reservationId: currentReservationId, serviceId: serviceId})
                        .done(function (response) {
                            if (response.success) {
                                console.log("Статус выполнения услуги обновлен:", serviceId);
                                showToast("Статус выполнения услуги обновлен", true);
                                loadReservationServices(currentReservationId); // Перезагрузить услуги
                            } else {
                                showToast(response.message, false);
                            }
                        })
                        .fail(function (xhr, status, error) {
                            console.error("Ошибка при обновлении статуса выполнения услуги:", error);
                            showToast("Ошибка при обновлении статуса выполнения услуги", false);
                        });
                });

                $('.remove-service').off('click').on('click', function () {
                    const serviceId = $(this).data('id');

                    $.post('/Home/RemoveServiceFromReservation', { reservationId: currentReservationId, serviceId: serviceId })
                        .done(function (response) {
                            if (response.success) {
                                console.log("Услуга удалена успешно:", serviceId);
                                showToast("Услуга успешно удалена", true);
                                loadReservationServices(currentReservationId); // Перезагрузить услуги
                            } else {
                                showToast(response.message, false);
                            }
                        })
                        .fail(function (xhr, status, error) {
                            console.error("Ошибка при удалении услуги:", error);
                            showToast("Ошибка при удалении услуги", false);
                        });
                });
            })
            .fail(function (xhr, status, error) {
                console.error("Ошибка при загрузке услуг:", error);
                showToast("Ошибка при загрузке услуг", false);
            });
    }


    // Добавление услуги к бронированию
    $('#addServiceButton').on('click', function () {
        const serviceId = $('#availableServices').val();
        if (!currentReservationId || !serviceId) {
            showToast("Ошибка: Недостаточно данных для добавления услуги", false);
            return;
        }

        $.post('/Home/AddServiceToReservation', { reservationId: currentReservationId, serviceId: serviceId })
            .done(function (response) {
                if (response.success) {
                    console.log("Услуга добавлена успешно:", serviceId);
                    showToast("Услуга успешно добавлена", true);
                    loadReservationServices(currentReservationId); // Обновляем данные
                } else {
                    showToast(response.message, false);
                }
            })
            .fail(function (xhr, status, error) {
                console.error("Ошибка при добавлении услуги:", error);
                showToast("Ошибка при добавлении услуги", false);
            });
    });

    // Загрузка доступных услуг
    function loadAvailableServices() {
        $.get('/Services/GetAll')
            .done(function (services) {
                const availableServices = $('#availableServices');
                availableServices.empty();

                services.forEach(service => {
                    availableServices.append(`
                        <option value="${service.Id}">${service.Title} (${service.Price} ₽)</option>
                    `);
                });
            })
            .fail(function (xhr, status, error) {
                console.error("Ошибка при загрузке доступных услуг:", error);
                showToast("Ошибка при загрузке доступных услуг", false);
            });
    }

    

    // Обновление таблицы после закрытия модального окна
    $('#servicesModal').on('hidden.bs.modal', function () {
        if (currentReservationId) {
            getClients()
            console.log("Таблица обновлена после закрытия модального окна.");
        }
    });

   
});
