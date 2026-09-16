import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnimalService } from '../../core/services/animal.service';
import { MlService } from '../../core/services/ml.service';
import { NotificationService } from '../../core/services/notification.service';
import { Animal, MilkHealth } from '../../core/models';
import { ANIMAL_STATUSES, ANIMAL_TYPES, GENDERS } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';

type AnimalForm = Partial<Animal>;

@Component({
  selector: 'app-animals',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './animals.component.html',
})
export class AnimalsComponent implements OnInit {
  private svc = inject(AnimalService);
  private ml = inject(MlService);
  private notify = inject(NotificationService);

  health = signal<Map<string, MilkHealth>>(new Map());

  statuses = ANIMAL_STATUSES;
  types = ANIMAL_TYPES;
  genders = GENDERS;

  animals = signal<Animal[]>([]);
  loading = signal(true);
  saving = signal(false);
  search = '';
  statusFilter = '';

  showForm = signal(false);
  editingId = signal<string | null>(null);
  form: AnimalForm = {};

  ngOnInit() {
    this.load();
    this.loadHealth();
  }

  load() {
    this.loading.set(true);
    this.svc.getAll(this.statusFilter || undefined, this.search || undefined).subscribe({
      next: a => { this.animals.set(a); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  private loadHealth() {
    this.ml.getHealthChecks(45).subscribe(list => {
      const map = new Map<string, MilkHealth>();
      list.forEach(h => map.set(h.animalId, h));
      this.health.set(map);
    });
  }

  healthFor(id: string): MilkHealth | undefined { return this.health().get(id); }

  healthBadge(status: string | undefined): string {
    switch (status) {
      case 'AtRisk': return 'low';
      case 'Watch': return 'pending';
      case 'Healthy': return 'ok';
      default: return 'gray';
    }
  }

  healthLabel(status: string | undefined): string {
    switch (status) {
      case 'AtRisk': return 'At risk';
      case 'Watch': return 'Watch';
      case 'Healthy': return 'Healthy';
      default: return '—';
    }
  }

  openCreate() {
    this.editingId.set(null);
    this.form = { status: 'Milking', type: 'Cow', gender: 'Female' };
    this.showForm.set(true);
  }

  openEdit(a: Animal) {
    this.editingId.set(a.animalId);
    this.form = { ...a };
    this.showForm.set(true);
  }

  close() { this.showForm.set(false); }

  save() {
    if (!this.form.tagNumber) { this.notify.warn('Tag number is required.'); return; }
    this.saving.set(true);
    const id = this.editingId();
    const done = (msg: string) => { this.notify.success(msg); this.saving.set(false); this.close(); this.load(); };
    const fail = () => this.saving.set(false);

    const f = this.form;
    const payload = {
      tagNumber: f.tagNumber!,
      name: f.name ?? null,
      type: f.type ?? null,
      breed: f.breed ?? null,
      gender: f.gender ?? null,
      dateOfBirth: f.dateOfBirth ?? null,
      purchaseDate: f.purchaseDate ?? null,
      purchasePrice: f.purchasePrice ?? null,
      status: f.status || 'Milking',
      dailyMilkYield: f.dailyMilkYield ?? null,
      photoPath: f.photoPath ?? null,
      notes: f.notes ?? null,
    };

    if (id) {
      this.svc.update(id, payload).subscribe({ next: () => done('Animal updated.'), error: fail });
    } else {
      this.svc.create(payload).subscribe({ next: () => done('Animal added.'), error: fail });
    }
  }

  remove(a: Animal) {
    if (!confirm(`Delete animal ${a.tagNumber}? This cannot be undone.`)) return;
    this.svc.delete(a.animalId).subscribe({ next: () => { this.notify.success('Animal deleted.'); this.load(); } });
  }

  statusClass(s: string): string {
    if (s === 'Sick' || s === 'Dead') return 'low';
    if (s === 'Sold') return 'gray';
    if (s === 'Pregnant') return 'info';
    return 'ok';
  }
}
