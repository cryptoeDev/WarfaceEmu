Warface Server Emulator (Source Leak & Release)

Server emulator for Warface build 1.22400.5519.45100. (20DEV)

Эмулятор WarFace под 2020 клиент, Dev клиента. Полностью рабочий, имеются админ-команды, отправка команд на сервер боя, 100% backend.
Полностью рабочие рейтинговые матчи. Остальное вы сможете увидеть сами. Исходник залит не полностью, если кому-то нужен, то обращайтесь в топике WFCryHub. 

Админ команды работают через Telegram-API. Все команды для бота и настройка хранятся в Config/api.json и Game/cache/BotsCmd.xml

Магазин меняется в Game/cache/shop_get_offers.xml

Поддерживает: 2017, 2020 Dev.

Quick start:

1. Compile it or download Release mode
2. Change data connect to your data-base (MySql) in Config/sql.json
3. Install .sql file to your MySql server
4. Start your Server (EmuWarface.exe)

Requirements: .NetCore 3.1, MySQL-Server


