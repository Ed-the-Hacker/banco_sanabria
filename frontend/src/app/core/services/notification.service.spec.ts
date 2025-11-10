import { TestBed } from '@angular/core/testing';
import { NotificationService, Notification } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should emit success notification', (done) => {
    const message = 'Operación exitosa';
    
    service.notification$.subscribe((notification: Notification) => {
      expect(notification.type).toBe('success');
      expect(notification.message).toBe(message);
      expect(notification.duration).toBe(3000);
      done();
    });

    service.success(message);
  });

  it('should emit error notification', (done) => {
    const message = 'Error al procesar';
    
    service.notification$.subscribe((notification: Notification) => {
      expect(notification.type).toBe('error');
      expect(notification.message).toBe(message);
      expect(notification.duration).toBe(5000);
      done();
    });

    service.error(message);
  });

  it('should emit warning notification', (done) => {
    const message = 'Advertencia importante';
    
    service.notification$.subscribe((notification: Notification) => {
      expect(notification.type).toBe('warning');
      expect(notification.message).toBe(message);
      expect(notification.duration).toBe(4000);
      done();
    });

    service.warning(message);
  });

  it('should emit info notification', (done) => {
    const message = 'Información relevante';
    
    service.notification$.subscribe((notification: Notification) => {
      expect(notification.type).toBe('info');
      expect(notification.message).toBe(message);
      expect(notification.duration).toBe(3000);
      done();
    });

    service.info(message);
  });

  it('should allow custom duration', (done) => {
    const message = 'Mensaje personalizado';
    const customDuration = 10000;
    
    service.notification$.subscribe((notification: Notification) => {
      expect(notification.duration).toBe(customDuration);
      done();
    });

    service.success(message, customDuration);
  });
});

