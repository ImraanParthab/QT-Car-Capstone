class Motor:
    def __init__(self, tb6612_driver):
        self.tb6612 = tb6612_driver

    def forward(self, speed=1.0):
        self.tb6612.set_speed(speed)
        self.tb6612.set_direction('forward')

    def backward(self, speed=1.0):
        self.tb6612.set_speed(speed)
        self.tb6612.set_direction('backward')

    def stop(self):
        self.tb6612.set_speed(0)

    def set_speed(self, speed):
        self.tb6612.set_speed(speed)