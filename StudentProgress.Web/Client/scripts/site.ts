import BaBarChart from "bootstrap-icons/icons/bar-chart-steps.svg?raw";
import QuickAddIcon from "bootstrap-icons/icons/clipboard2-plus.svg?raw";
import GroupsIcon  from "bootstrap-icons/icons/person-lines-fill.svg?raw";
import SettingsIcon from "bootstrap-icons/icons/gear.svg?raw";


(document.querySelector('#brandSvg') as HTMLDivElement).innerHTML = BaBarChart;
(document.querySelector('#quickAddSvg') as HTMLDivElement).innerHTML = QuickAddIcon;
(document.querySelector('#groupsSvg') as HTMLDivElement).innerHTML = GroupsIcon;
(document.querySelector('#settingsSvg') as HTMLDivElement).innerHTML = SettingsIcon;
console.log('hello world');