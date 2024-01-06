INSERT INTO elements
    (name)
VALUES
    ('fire'),
    ('water'),
    ('earth'),
    ('air'),
    ('shadow'),
    ('light');

INSERT INTO cardtypes
    (name)
VALUES
    ('monster'),
    ('spell');

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


INSERT INTO users (username, password, bio, image)
VALUES ('Alice', 'password', 'alice-bio', 'alice-image'),
       ('Bob', 'password', 'bob-bio', 'bob-image');

INSERT INTO tokens (guid, userid)
VALUES (
    'alice-token',
    (SELECT id FROM users WHERE username = 'Alice')
),
(
    'bob-token',
    (SELECT id FROM users WHERE username = 'Bob')
);
