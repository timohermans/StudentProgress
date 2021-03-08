window.initStudentNameController = function() {
    return {
        isEditing: false,
        startEditing() {
            this.isEditing = true;
            setTimeout(() => {
                const nameElement = this.$refs.nameInput;
                nameElement.focus();
                nameElement.setSelectionRange(nameElement.value.length, nameElement.value.length);
            });
        },
        cancelEditing() {
            this.$refs.nameDisplay.value = this.$refs.nameOriginal.value;
            this.isEditing = false;
        },
        updateName(evt) {
            evt.preventDefault();
            const data = new FormData(this.$refs.form);
            if (data.get('name') === this.$refs.nameOriginal.value) {
                this.cancelEditing();
                return;
            }
            
            fetch(`${window.applicationBaseUrl}api/student/${data.get('id')}/name`, {
                method: 'PUT',
                body: data
            }).then((res) => {
                if (res.ok) {
                    this.$refs.nameDisplay.innerHTML = data.get('name');
                    this.isEditing = false;
                } else {
                    alert(`Something went wrong editing the name`);
                    this.cancelEditing();
                }
            });
        }
    }
}