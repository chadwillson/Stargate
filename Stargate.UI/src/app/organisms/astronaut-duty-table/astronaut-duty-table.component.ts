import { Component, Input } from '@angular/core';
import { AstronautDuty } from '../../shared/models';

@Component({
  selector: 'app-astronaut-duty-table',
  templateUrl: './astronaut-duty-table.component.html',
  styleUrls: ['./astronaut-duty-table.component.scss'],
  standalone: false
})
export class AstronautDutyTableComponent {
  @Input() duties: AstronautDuty[] = [];
  @Input() loading = false;
}
