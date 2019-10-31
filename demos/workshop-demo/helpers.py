class SlidingAverage:
    def __init__(self, window_size):
        self.index = 0
        self.values = [0] * window_size

    def _previous(self):
        return self.values[(self.index + len(self.values) - 1) % len(self.values)]

    def update(self, value):
        self.values[self.index] = self._previous() + value
        self.index = (self.index + 1) % len(self.values)

    def get(self):
        return (self._previous() - self.values[self.index]) / (len(self.values) - 1)