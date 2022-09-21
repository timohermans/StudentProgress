﻿function hotkeyController() {
    return {
        listenToKeys() {
            document.onkeyup = (e) => {
                if (['input', 'textarea'].includes(document.activeElement.tagName.toLowerCase())) return;
                
                const feelingKeys = ["!", "@", "#"];
                if (feelingKeys.includes(e.key)) {
                    const keySelected = (feelingKeys.indexOf(e.key) + 1).toString();
                    const option = [...this.$refs.feeling.options].find(o => o.value === keySelected);
                    if (option) this.$refs.feeling.value = option.value;
                }
                
                if (e.key === "R") {
                    this.$refs.isReviewed.checked = !this.$refs.isReviewed.checked;
                }
                
            };
        },
        setFeeling(event) {
            const mileStoneRating = event.target.value;
            
            let feeling;
            if (["Undefined", "Orienting"].includes(mileStoneRating)){
                feeling = 1;
            } else if (mileStoneRating === "Beginning") {
                feeling = 2;
            } else {
                feeling = 3;
            }
            
            this.$refs.feeling.value = feeling;
        }
    };
}