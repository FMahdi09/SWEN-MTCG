-- users

CREATE TABLE IF NOT EXISTS users(
    id SERIAL NOT NULL UNIQUE,
    username VARCHAR NOT NULL,
    password VARCHAR NOT NULL,
    bio VARCHAR NOT NULL,
    image VARCHAR NOT NULL,
    currency INT NOT NULL DEFAULT 20
);

-- tokens

CREATE TABLE IF NOT EXISTS tokens(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userid INT NOT NULL,
    -- constraints
    CONSTRAINT fkUserId
        FOREIGN KEY(userid)
            REFERENCES users(id)
);

-- elements

CREATE TABLE IF NOT EXISTS elements(
    id SERIAL NOT NULL UNIQUE,
    name VARCHAR NOT NULL
);

INSERT INTO elements
    (name)
VALUES
    ('fire'),
    ('water'),
    ('earth'),
    ('air'),
    ('shadow'),
    ('light');

-- cardtypes

CREATE TABLE IF NOT EXISTS cardtypes(
    id SERIAL NOT NULL UNIQUE,
    name VARCHAR NOT NULL
);

INSERT INTO cardtypes
    (name)
VALUES
    ('monster'),
    ('spell');

-- cards

CREATE TABLE IF NOT EXISTS cards(
    id SERIAL NOT NULL UNIQUE,
    elementid INT NOT NULL,
    cardtypeid INT NOT NULL,
    damage INT NOT NULL,
    name VARCHAR NOT NULL,
    -- constraints
    CONSTRAINT fkElementId
        FOREIGN KEY(elementid)
            REFERENCES elements(id),
    CONSTRAINT fkCardtypeId
        FOREIGN KEY(cardtypeid)
            REFERENCES cardtypes(id)
);

INSERT INTO cards
    (elementid, cardtypeid, damage, name)
VALUES
    (
        (SELECT id FROM elements WHERE name = 'fire'),
        (SELECT id FROM cardtypes WHERE name = 'monster'),
        100,
        'Dragon'
    ),
    (
        (SELECT id FROM elements WHERE name = 'fire'),
        (SELECT id FROM cardtypes WHERE name = 'spell'),
        60,
        'Fireball'
    ),
    (
        (SELECT id FROM elements WHERE name = 'water'),
        (SELECT id FROM cardtypes WHERE name = 'monster'),
        50,
        'WaterSpirit'
    ),
    (
        (SELECT id FROM elements WHERE name = 'water'),
        (SELECT id FROM cardtypes WHERE name = 'spell'),
        40,
        'Waterfall'
    ),
    (
        (SELECT id FROM elements WHERE name = 'earth'),
        (SELECT id FROM cardtypes WHERE name = 'monster'),
        80,
        'Golem'
    ),
    (
        (SELECT id FROM elements WHERE name = 'earth'),
        (SELECT id FROM cardtypes WHERE name = 'spell'),
        110,
        'Earthquake'
    ),
    (
        (SELECT id FROM elements WHERE name = 'air'),
        (SELECT id FROM cardtypes WHERE name = 'monster'),
        55,
        'DustDevil'
    ),
    (
        (SELECT id FROM elements WHERE name = 'air'),
        (SELECT id FROM cardtypes WHERE name = 'spell'),
        30,
        'Tornado'
    ),
    (
        (SELECT id FROM elements WHERE name = 'shadow'),
        (SELECT id FROM cardtypes WHERE name = 'monster'),
        70,
        'Stalker'
    ),
    (
        (SELECT id FROM elements WHERE name = 'shadow'),
        (SELECT id FROM cardtypes WHERE name = 'spell'),
        80,
        'Shadowball'
    );

-- createdcards

CREATE TABLE IF NOT EXISTS createdcards(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userid INT NOT NULL,
    cardid INT NOT NULL,
    deck BOOL DEFAULT false,
    -- constraints
    CONSTRAINT fkUserId
        FOREIGN KEY(userid)
            REFERENCES users(id),
    CONSTRAINT fkCardId
        FOREIGN KEY(cardid)
            REFERENCES cards(id)
);