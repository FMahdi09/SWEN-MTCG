INSERT INTO users (username, password, bio, image)
VALUES ('Alice', 'password', 'test-bio', 'test-image');

INSERT INTO tokens (guid, userid)
VALUES (
    'test-token',
    (SELECT id from users WHERE username = 'Alice')
);