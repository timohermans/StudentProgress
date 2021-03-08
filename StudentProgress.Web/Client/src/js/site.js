// 3rd party js
import 'bootstrap';
import 'alpinejs';
// 3rd party css
import 'bootstrap/dist/css/bootstrap.css';

// custom js
import '../js/search';
// custom css
import '../css/site.css';

console.log('the \'site\' bundle has been loaded!');

function autoSizeTextAreas() {
    document.querySelectorAll('textarea').forEach(element => {
        element.setAttribute('style', `height:${element.scrollHeight}px;overflow-y:hidden`);
        element.addEventListener('input', event => {
            event.target.style.height = 'auto';
            event.target.style.height = `${event.target.scrollHeight}px`;
        });
    });
}

function manuallyResizeTextArea(textArea) {
    textArea.dispatchEvent(new Event('input'));
}

function preventUnsavedChanges() {
    let isFormChanged = false;
    let isSubmitting = false;
    const form = document.querySelector('form');
    if (!form) return;
    form.addEventListener('submit', () => {
        isSubmitting = true;
        isFormChanged = false;
    });
    form.addEventListener('input', () => {
        isFormChanged = true;
    });

    window.onbeforeunload = function() {
        if (isFormChanged && !isSubmitting) {
            return "";
        }
    }
}

preventUnsavedChanges();
autoSizeTextAreas();