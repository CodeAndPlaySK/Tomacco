PRAGMA foreign_keys = ON;

--------------------------------------------------
-- PLAYERS
--------------------------------------------------
CREATE TABLE Users (
    telegram_id TEXT PRIMARY KEY,
    username TEXT NOT NULL,
);

CREATE INDEX idx_user_name ON Users(username);

--------------------------------------------------
-- FAMILIES
--------------------------------------------------
CREATE TABLE Families (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    gold INTEGER NOT NULL DEFAULT 0,
    influence INTEGER NOT NULL DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_families_name ON Families(name);

--------------------------------------------------
-- FAMILIES OF PLAYERS
--------------------------------------------------

CREATE TABLE FamiliesOfPlayers (
    id_family INTEGER PRIMARY KEY,
    telegram_id TEXT PRIMARY KEY,
    FOREIGN KEY (id_family) REFERENCES Families(id),
    FOREIGN KEY (telegram_id) REFERENCES Users(telegram_id)
);

--------------------------------------------------
-- HERO
--------------------------------------------------
CREATE TABLE hero (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    families_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    strength INTEGER NOT NULL DEFAULT 0,
    mind INTEGER NOT NULL DEFAULT 0,
    faith INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (families_id) REFERENCES Families(id)
);

CREATE INDEX idx_hero_families ON hero(families_id);

--------------------------------------------------
-- BUILDING
--------------------------------------------------
CREATE TABLE building (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    code TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL,
    gold_cost INTEGER NOT NULL,
    owner_Families_id INTEGER,
    FOREIGN KEY (owner_Families_id) REFERENCES Families(id)
);

CREATE INDEX idx_building_code ON building(code);
CREATE INDEX idx_building_owner ON building(owner_Families_id);

--------------------------------------------------
-- BUILDING HERO SLOTS (runtime occupation)
--------------------------------------------------
CREATE TABLE building_hero_slot (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    building_id INTEGER NOT NULL,
    hero_id INTEGER NOT NULL,
    turn_start INTEGER NOT NULL,
    turn_end INTEGER,
    FOREIGN KEY (building_id) REFERENCES building(id),
    FOREIGN KEY (hero_id) REFERENCES hero(id)
);

CREATE INDEX idx_slot_building ON building_hero_slot(building_id);
CREATE INDEX idx_slot_hero ON building_hero_slot(hero_id);

--------------------------------------------------
-- BUILDING ACTION (definition)
--------------------------------------------------
CREATE TABLE building_action (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    building_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    duration_turns INTEGER NOT NULL,
    FOREIGN KEY (building_id) REFERENCES building(id)
);

CREATE INDEX idx_building_action_building ON building_action(building_id);

--------------------------------------------------
-- ACTIVE BUILDING ACTION (runtime instance)
--------------------------------------------------
CREATE TABLE active_building_action (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    building_action_id INTEGER NOT NULL,
    hero_id INTEGER NOT NULL,
    Families_id INTEGER NOT NULL,
    turn_start INTEGER NOT NULL,
    turn_end INTEGER NOT NULL,
    FOREIGN KEY (building_action_id) REFERENCES building_action(id),
    FOREIGN KEY (hero_id) REFERENCES hero(id),
    FOREIGN KEY (Families_id) REFERENCES Families(id)
);

CREATE INDEX idx_active_action_turn_end ON active_building_action(turn_end);

--------------------------------------------------
-- RESOURCE EVENT (event-sourcing light)
--------------------------------------------------
CREATE TABLE resource_event (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    Families_id INTEGER NOT NULL,
    resource_type TEXT NOT NULL,
    amount INTEGER NOT NULL,
    turn INTEGER NOT NULL,
    source TEXT,
    FOREIGN KEY (Families_id) REFERENCES Families(id)
);

CREATE INDEX idx_resource_event_Families ON resource_event(Families_id);
CREATE INDEX idx_resource_event_turn ON resource_event(turn);

--------------------------------------------------
-- EVENT LOG (audit / replay / debugging)
--------------------------------------------------
CREATE TABLE game_event (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_type TEXT NOT NULL,
    payload TEXT NOT NULL,
    turn INTEGER NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_game_event_type ON game_event(event_type);
CREATE INDEX idx_game_event_turn ON game_event(turn);
