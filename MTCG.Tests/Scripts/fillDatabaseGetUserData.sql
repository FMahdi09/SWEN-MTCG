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