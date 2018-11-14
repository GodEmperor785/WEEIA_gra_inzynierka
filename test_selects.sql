select version();

SHOW VARIABLES LIKE "%version%";

use gamedatabase;

select * from gamedatabase.factions; 

select * from gamedatabase.players;

select * from gamedatabase.ships;

select * from gamedatabase.weapons;

select * from gamedatabase.defence_systems;

select * from gamedatabase.shiptemplates_weapons;

select * from gamedatabase.shiptemplates_defencesystems;

select * from gamedatabase.fleets_ships;

select * from gamedatabase.ship_templates;

select * from gamedatabase.lootboxes;

select * from gamedatabase.fleets;

select * from gamedatabase.game_history;

SHOW PROCESSLIST;

show status like '%onn%';
-- Connection to suma wszystkich kiedykolwiek
-- Threads_connected to otwarte
