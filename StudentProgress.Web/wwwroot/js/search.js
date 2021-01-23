function initSearch() {
    return {
        searchResults: [],
        search(event) {
            determineWhetherToSearch(event.target.value)
                .then(doASearch)
                .then(data => {
                    this.searchResults = data;
                    showPopup();
                })
                .catch(() => {
                    this.searchResults = [];
                });
        }
    }
}

function determineWhetherToSearch(searchInput) {
    return new Promise((resolve, reject) => {
        const amountOfCharacters = searchInput.length;

        if (amountOfCharacters >= 3) {
            resolve(searchInput);
        }

        reject();
    });
}

function doASearch(searchInput) {
    return fetch(`${window.applicationBaseUrl}api/search/${searchInput}`)
        .then(res => res.json());
}

function showPopup() {
    const tooltip = Popper.createPopper(
        document.querySelector('#search'),
        document.querySelector('#search-results'), {
            placement: 'bottom',
            modifiers: [
                {
                    name: 'offset',
                    options: {
                        offset: [0, 8],
                    },
                },
            ],
        });
    // because initial render doesn't position correctly, update again
    setTimeout(() => tooltip.update(), 0); 
}
