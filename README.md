Для запуска проекта используйте docker-compose.yml

В docker-compose.yml устанавливаются:
1) MongoDb
2) Mongo client
3) Weather API (При старте приложения будет запущен Parser. Нужно ~ 2 минутки на заполение данных с сайта https://www.gismeteo.ru/)
4) Weather App

После успешного запуска контейнеров вы можете воспользоваться:
1) Web API : http://localhost:8000/swagger
2) Web App : http://localhost:8001