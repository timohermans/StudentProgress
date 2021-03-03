function initMilestoneTableActionsController() {
    return {
        isSelecting: false,
        milestonesSelected: [],
        learningOutcomeNew: '',
        useLearningOutcomeAsNewName(evt) {
          if (!this.isSelecting) return;
          
          this.learningOutcomeNew = evt.target.innerText;
        },
        startSelecting() {
            this.isSelecting = true;
        },
        stopSelecting() {
            this.milestonesSelected = [];
            this.learningOutcomeNew = '';
            this.isSelecting = false;
        },
        getMilestoneInputNameFor(index) {
            return `milestoneIds[${index}]`;
        }
    };
}