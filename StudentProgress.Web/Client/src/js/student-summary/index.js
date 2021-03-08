import '../../css/summary.css';

window.studentInfoController = function () {
    return {
        isEditing: false,
        isSaving: false,
        startEditing() {
            this.isEditing = true;
            setTimeout(() => {
                const noteElement = this.$refs.noteInput;
                noteElement.focus();
                noteElement.setSelectionRange(noteElement.value.length, noteElement.value.length);
            }, 0);
        },
        cancelEditing() {
          this.$refs.noteInput.value = this.$refs.originalNote.value;
          this.isEditing = false;
        },
        save(evt) {
            evt.preventDefault();
            const data = new FormData(this.$refs.form);
            this.isSaving = true;
            fetch(`${window.applicationBaseUrl}api/student/${data.get('id')}`, {body: data, method: 'PUT'})
                .then((res) => {
                    this.$refs.displayNote.innerHTML = data.get('note');
                    this.isSaving = false;
                    this.isEditing = false;
                });
        }
    }
}