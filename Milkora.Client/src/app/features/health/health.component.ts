import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HealthService } from '../../core/services/health.service';
import { AnimalService } from '../../core/services/animal.service';
import { NotificationService } from '../../core/services/notification.service';
import { HealthRecord, Animal, CreateHealthRecordRequest } from '../../core/models';
import { HEALTH_RECORD_TYPES } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { today } from '../../shared/utils/dates';

@Component({
  selector: 'app-health',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './health.component.html',
})
export class HealthComponent implements OnInit {
  private svc = inject(HealthService);
  private animalSvc = inject(AnimalService);
  private notify = inject(NotificationService);

  types = HEALTH_RECORD_TYPES;
  typeFilter = '';
  animalFilter = '';

  records = signal<HealthRecord[]>([]);
  animals = signal<Animal[]>([]);
  loading = signal(true);
  saving = signal(false);
  showForm = signal(false);
  form: Partial<CreateHealthRecordRequest> = {};

  ngOnInit() {
    this.animalSvc.getAll().subscribe(a => this.animals.set(a));
    this.load();
  }

  load() {
    this.loading.set(true);
    this.svc.getByAnimal(this.animalFilter || undefined, this.typeFilter || undefined).subscribe({
      next: r => { this.records.set(r); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() { this.form = { recordDate: today(), recordType: 'Vaccine' }; this.showForm.set(true); }
  close() { this.showForm.set(false); }

  save() {
    this.saving.set(true);
    this.svc.create({
      recordDate: this.form.recordDate || today(),
      recordType: this.form.recordType || 'Checkup',
      animalId: this.form.animalId || null,
      medicineName: this.form.medicineName ?? null,
      vetName: this.form.vetName ?? null,
      vetContact: this.form.vetContact ?? null,
      cost: this.form.cost ?? null,
      nextDueDate: this.form.nextDueDate ?? null,
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Health record saved.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }
}
