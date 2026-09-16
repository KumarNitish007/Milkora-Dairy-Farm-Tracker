import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BreedingService } from '../../core/services/breeding.service';
import { AnimalService } from '../../core/services/animal.service';
import { NotificationService } from '../../core/services/notification.service';
import { BreedingRecord, Animal, CreateBreedingRequest } from '../../core/models';
import { PREGNANCY_STATUSES } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { today } from '../../shared/utils/dates';

@Component({
  selector: 'app-breeding',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './breeding.component.html',
})
export class BreedingComponent implements OnInit {
  private svc = inject(BreedingService);
  private animalSvc = inject(AnimalService);
  private notify = inject(NotificationService);

  statuses = PREGNANCY_STATUSES;
  animalFilter = '';

  records = signal<BreedingRecord[]>([]);
  animals = signal<Animal[]>([]);
  loading = signal(true);
  saving = signal(false);

  showForm = signal(false);
  form: Partial<CreateBreedingRequest> = {};

  // calving modal
  showCalving = signal(false);
  calvingFor = signal<BreedingRecord | null>(null);
  calving = { calvingDate: today(), calfDetails: '', pregnancyStatus: 'Delivered' };

  ngOnInit() {
    this.animalSvc.getAll().subscribe(a => this.animals.set(a));
    this.load();
  }

  load() {
    this.loading.set(true);
    this.svc.getByAnimal(this.animalFilter || undefined).subscribe({
      next: r => { this.records.set(r); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() { this.form = { inseminationDate: today(), pregnancyStatus: 'Inseminated' }; this.showForm.set(true); }
  close() { this.showForm.set(false); }

  save() {
    this.saving.set(true);
    this.svc.create({
      inseminationDate: this.form.inseminationDate || today(),
      animalId: this.form.animalId || null,
      bullDetails: this.form.bullDetails ?? null,
      pregnancyStatus: this.form.pregnancyStatus || 'Inseminated',
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Breeding record saved.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  openCalving(r: BreedingRecord) {
    this.calvingFor.set(r);
    this.calving = { calvingDate: today(), calfDetails: '', pregnancyStatus: 'Delivered' };
    this.showCalving.set(true);
  }
  closeCalving() { this.showCalving.set(false); }

  saveCalving() {
    const r = this.calvingFor();
    if (!r) return;
    this.saving.set(true);
    this.svc.updateCalving(r.breedingId, { ...this.calving }).subscribe({
      next: () => { this.notify.success('Calving recorded.'); this.saving.set(false); this.closeCalving(); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  statusClass(s: string): string {
    if (s === 'Delivered') return 'ok';
    if (s === 'Confirmed') return 'info';
    if (s === 'NotPregnant') return 'low';
    return 'pending';
  }
}
