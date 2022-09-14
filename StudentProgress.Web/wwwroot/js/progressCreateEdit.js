function hotkeyController() {
    return {
        message: "hello folks!",
        listenToKeys() {
            document.onkeyup = (e) => {
                if (['input', 'textarea'].includes(document.activeElement.tagName.toLowerCase())) return;
                
                const feelingKeys = ["!", "@", "#"];
                if (feelingKeys.includes(e.key)) {
                    const keySelected = (feelingKeys.indexOf(e.key) + 1).toString();
                    const option = [...this.$refs.feelingSelect.options].find(o => o.value === keySelected);
                    if (option) this.$refs.feelingSelect.value = option.value;
                }
                
            };
        }
    };
}