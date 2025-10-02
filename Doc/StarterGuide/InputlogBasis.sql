CREATE TABLE tasks (
  id INT PRIMARY KEY AUTO_INCREMENT,
  fkID_user VARCHAR(24) NOT NULL,
  time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  status INT NOT NULL,
  guid VARCHAR(128) NOT NULL,
  downloaded TINYINT(1) DEFAULT 0
);

CREATE TABLE analyses (
  id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(256) NOT NULL,
  active TINYINT(1) NOT NULL
);

CREATE TABLE task_analyses (
  id INT PRIMARY KEY AUTO_INCREMENT,
  fkID_task INT NOT NULL,
  fkID_analysis INT NOT NULL,
  FOREIGN KEY (fkID_task) REFERENCES tasks(id),
  FOREIGN KEY (fkID_analysis) REFERENCES analyses(id)
);

INSERT INTO `my_aspnet_roles` (`id`, `applicationId`, `name`) VALUES
(1, 1, 'Admin'),
(2, 1, 'Default'),
(3, 1, 'Disabled');