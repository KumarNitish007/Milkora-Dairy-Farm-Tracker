import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MilkLogService } from '../../core/services/milk-log.service';
import { AnimalService } from '../../core/services/animal.service';
import { NotificationService } from '../../core/services/notification.service';
import { MilkLog, Animal, CreateMilkLogRequest } from '../../core/models';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { today } from '../../shared/utils/dates';

@Component({
  selector: 'app-milk-log',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './milk-log.component.html',
})
export class MilkLogComponent implements OnInit {
  private svc = inject(MilkLogService);
  private animalSvc = inject(AnimalService);
  private notify = inject(NotificationService);

  date = today();
  logs = signal<MilkLog[]>([]);
  animals = signal<Animal[]>([]);
  loading = signal(true);
  saving = signal(false);

  showForm = signal(false);
  form: Partial<CreateMilkLogRequest> = {};

  totalMilk = computed(() => this.logs().reduce((s, l) => s + l.totalMilk, 0));
  totalValue = computed(() => this.logs().reduce((s, l) => s + l.totalValue, 0));

  ngOnInit() {
    this.animalSvc.getAll().subscribe(a => this.animals.set(a));
    this.load();
  }

  load() {
    this.loading.set(true);
    this.svc.getByDate(this.date).subscribe({
      next: l => { this.logs.set(l); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() {
    this.form = { logDate: this.date, morningMilk: 0, eveningMilk: 0, ratePerLitre: undefined };
    this.showForm.set(true);
  }
  close() { this.showForm.set(false); }

  save() {
    this.saving.set(true);
    const req: CreateMilkLogRequest = {
      logDate: this.form.logDate || this.date,
      animalId: this.form.animalId || null,
      morningMilk: Number(this.form.morningMilk) || 0,
      eveningMilk: Number(this.form.eveningMilk) || 0,
      fatPercent: this.form.fatPercent ?? null,
      ratePerLitre: this.form.ratePerLitre ?? null,
      notes: this.form.notes ?? null,
    };
    this.svc.create(req).subscribe({
      next: () => { this.notify.success('Milk log saved.'); this.saving.set(false); this.close(); this.date = req.logDate; this.load(); },
      error: () => this.saving.set(false),
    });
  }
}
