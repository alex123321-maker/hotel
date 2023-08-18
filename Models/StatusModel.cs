﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class StatusModel
    {
        [Key]
        public string Title { get; set; }
    }
}
// 1. Ожидание подтверждения (Pending Confirmation):
// Это начальный статус, когда клиент сделал запрос на бронирование, но оно еще не подтверждено отелем.
// Этот статус может означать, что отель проверяет наличие свободных номеров или ресурсов.

// 2. Подтверждено (Confirmed):
// Бронирование успешно подтверждено отелем. Клиенту гарантировано место или услуга в указанные даты.
// Это фактическое бронирование.

// 3. Отменено (Canceled):
// Бронирование было отменено клиентом или отелем.
// Этот статус может возникнуть, если клиент передумал или в случае несоответствия условий бронирования.

// 4. В процессе (In Progress):
// Этот статус может использоваться для бронирований, которые еще не начались, но уже подтверждены.
// Например, если бронирование на будущие даты, но даты еще не наступили.

// 5. Завершено (Completed):
// Бронирование успешно завершено, клиент прожил свой срок или воспользовался услугами.

// 6. Неявка (No Show):
// Клиент не явился на бронированное место или не воспользовался услугой, не предупредив отель.

// 7. Отклонено (Declined):
// Бронирование отклонено отелем из-за различных причин, таких как отсутствие мест или невозможность выполнения запроса.

// 8. В ожидании оплаты (Pending Payment):
// Бронирование ожидает оплаты от клиента. Отель может подтвердить бронирование после получения оплаты.
