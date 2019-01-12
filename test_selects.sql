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

select s.Name, w.Name from gamedatabase.ship_templates as s, gamedatabase.weapons as w, gamedatabase.shiptemplates_weapons as sw
	where s.Id = sw.ShipTemplateID and w.Id = sw.WeaponID;
    
select s.Name, d.Name from gamedatabase.ship_templates as s, gamedatabase.defence_systems as d, gamedatabase.shiptemplates_defencesystems as sd
	where s.Id = sd.ShipTemplateID and d.Id = sd.DefenceSystemID order by s.Name;

select * from gamedatabase.lootboxes;

select * from gamedatabase.fleets;

select * from gamedatabase.game_history;

SHOW PROCESSLIST;

show status like '%onn%';
-- Connection to suma wszystkich kiedykolwiek
-- Threads_connected to otwarte

select VERSION();