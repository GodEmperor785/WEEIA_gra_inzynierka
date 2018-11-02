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

SHOW PROCESSLIST;

show status like '%onn%';
-- Connection to suma wszystkich kiedykolwiek
-- Threads_connected to otwarte
