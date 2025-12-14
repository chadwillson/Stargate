import { Component, Input } from '@angular/core';
import { AstronautDutyRecord } from '../../shared/models';

@Component({
  selector: 'app-duty-history-detail',
  standalone: false,
  templateUrl: './duty-history-detail.component.html',
  styleUrl: './duty-history-detail.component.scss'
})
export class DutyHistoryDetailComponent {
  @Input() personName = '';
  @Input() duties: AstronautDutyRecord[] = [];
}
