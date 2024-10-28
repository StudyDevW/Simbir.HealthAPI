# Основное задание: 
1. Account URL: http://localhost:8081/ui-swagger 
2. Hospital URL: http://localhost:8082/ui-swagger 
3. Timetable URL: http://localhost:8083/ui-swagger 
4. Document URL: http://localhost:8084/ui-swagger

# Дополнительная информация

- На каждом микросервисе реализован редирект из корня "/" к "/ui-swagger"
  
- Связь между микросервисами реализована через "HTTP" метод
  
- База данных PostgreSQL
  
- Панель управления базой данных PgAdmin4: http://localhost:5050/
  
- JWT токены подписаны RS512 с публичным и приватным ключом RSA
  
- Использовал для хранения JWT токенов и их управлением, Redis

- Не получилось сделать дефолтные значения в процессе миграции, поэтому вот
оставляю команду 
- INSERT INTO "userTableObj" (id, "firstName", "lastName", "username", "password", "roles") VALUES
- (1, 'Администратор', NULL, 'admin', 'admin', ARRAY['Admin']),
- (2, 'Менеджер', NULL, 'manager', 'manager', ARRAY['Manager']),
- (3, 'Доктор', NULL, 'doctor', 'doctor', ARRAY['Doctor']),
- (4, 'Пользователь', NULL, 'user', 'user', ARRAY['User']);
