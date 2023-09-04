$(document).ready(function () {

    const clientInput = document.getElementById('clientInput');
    const autocompleteList = document.getElementById('autocompleteList');
    const searchButton = document.getElementById('searchButton');
    let words = []
    clients.forEach(client => {
        obj = {
            "value": client,
            "words": `ФИО: ${client.Surname} ${client.Name} ${client.Lastname} | Телефон: ${client.Phone} | Паспорт: ${client.Passportseries} ${client.Passportnumber} `
        }
        words.push(obj) 
    });
    clientInput.addEventListener('input', function () {
        const inputText = this.value.toLowerCase();
        autocompleteList.innerHTML = '';

        if (inputText.length === 0) {
            autocompleteList.style.display = 'none';
            return;
        }

        const matchedWords = words.filter(word => word.words.toLowerCase().includes(inputText));

        if (matchedWords.length === 0) {
            autocompleteList.style.display = 'none';
        }

        autocompleteList.style.display = 'block';
        matchedWords.forEach(word => {
            const li = document.createElement('li');
            li.textContent = word.words;
            li.classList.add('dropdown-item');
            li.addEventListener('click', function () {
                clientInput.value = word.words;
                clientInput.setAttribute("data", word.value.Id);
                autocompleteList.style.display = 'none';
            });
            autocompleteList.appendChild(li);
        });
        const li = document.createElement('li');
        const btn = document.createElement('button');
        btn.classList.add("new-client-btn");
        btn.classList.add("btn");
        btn.classList.add("btn-outline-primary");
        btn.textContent = "Добавить клиента";
        li.appendChild(btn);
        autocompleteList.appendChild(li);
    });

    //searchButton.addEventListener('click', function() {
    //  const inputText = searchInput.value;
    //  alert(`Perform search for: ${inputText}`);
    //});

    document.addEventListener('click', function (event) {
        if (!autocompleteList.contains(event.target)) {
            autocompleteList.style.display = 'none';
        }
    });
});