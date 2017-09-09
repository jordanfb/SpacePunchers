--[[
This is the prototype for the game
Literally just as many joysticks as are connected get a fist to punch around with.
They may randomize tints when joining, just for ease of seeing who you are.
Punch each other!
Maybe other enemies?
]]--


ProtoFist = {}

function ProtoFist:new(args)
	local object = {}
	setmetatable(object, self)
	self.__index = self
	if args ~= nil then
		object:init(args)
	end
	return object
end

function ProtoFist:init(args)
	self.joystick = args.joystick
	self.color = args.color or {255, 255, 255, 255}
	self.loc = {x = 0, y = 0, dx = 0, dy = 0, f = 0, df = 0}
	self.boostLevel = 0
	self.boostCountdown = 0 -- when this is greater than 0, you MEGA boost!
	self.boostAngle = 0 -- set when you start charging up boost, if you get too far away, the boost fails
	self.buttonStatus = {x = 0, y = 0, a = 0, b = 0, rtrigger = 0, ltrigger = 0, leftx = 0, lefty = 0, rightx = 0, righty = 0}
	--{leftFlame = false, rightFlame = false, mainFlame = false, }

	self.drawVars = {flameTimer = 0, flameFrameTime = 0.2, flipVars = {main = 1, left = 1, right = -1}} -- offset the side flames by like .1 seconds so they don't obviously change at the same time
	self.images = {fist = love.graphics.newImage("fist.png"), mainFlame = love.graphics.newImage("flame1.png"), leftFlame = love.graphics.newImage("left flame.png"),
									rightFlame = love.graphics.newImage("right flame.png")}
end

function ProtoFist:gamepadpressed(gamepad, button)
	self.buttonStatus[button] = 1
end

function ProtoFist:gamepadreleased(gamepad, button)
	self.buttonStatus[button] = 0
end

function ProtoFist:gamepadaxis(gamepad, axis, value)
	self.buttonStatus[axis] = value
	print(axis)
end

function ProtoFist:update(dt)
	self:physicsUpdate(dt)
	self:graphicsUpdate(dt)
end

function ProtoFist:draw()
	love.graphis.setColor(self.color)
	love.graphics.draw(self.images.fist, self.loc.x, self.loc.y, 1, 1, self.images.fist:getWidth()/2, self.images.fist:getHeight()/2)
	love.graphics.setColor(255, 255, 255, 255) -- then draw the flames regular color
	-- or don't, if they aren't firing...
end

function ProtoFist:drawRelative(image, dx, dy, xscale, yscale)
	-- draws the image relative to the ship location and facing to make it easier to draw flames
	local sin = math.sin(self.loc.f)
	local cos = math.cos(self.loc.f)
	local x = self.loc.x + sin*dx + cos*dy
	local y = self.loc.y + cos*dx + sin*dy
	love.graphics.draw(image, x, y, self.loc.f, xscale, yscale, image:getWidth()/2, image:getHeight()/2)
end

function ProtoFist:physicsUpdate(dt)
	self.loc.x = self.loc.x + self.loc.dx * dt
	self.loc.y = self.loc.y + self.loc.dy * dt
	self.loc.f = self.loc.f + self.loc.df * dt
end

function ProtoFist:graphicsUpdate(dt)
	self.drawVars.flameTimer = self.drawVars.flameTimer + dt
	if self.drawVars.flameTimer > self.drawVars.flameFrameTime then
		self.drawVars.flameTimer = self.drawVars.flameTimer - self.drawVars.flameFrameTime
		-- and we flip whatever needs to be flipped.
		self.drawVars.flipVars.main = self.drawVars.flipVars.main * -1
		self.drawVars.flipVars.left = self.drawVars.flipVars.left * -1
		self.drawVars.flipVars.right = self.drawVars.flipVars.right * -1
	end
end

function ProtoFist:drawFlames()
	--
end



ProtoGame = {}

function ProtoGame:new(args)
	local object = {}
	setmetatable(object, self)
	self.__index = self
	if args ~= nil then
		object:init(args)
	end
	return object
end

function ProtoGame:init(args)
	self.players = {} -- when a joystick is added, then it makes a new player, indexed by numbers, probably?
end

function ProtoGame:joystickadded(joystick)
	self.players[joystick] = ProtoFist:new()
	self.players[joystick]:init()
end

function ProtoGame:joystickremoved(joystick)
	self.players[joystick] = nil -- just delete it for now
end

function ProtoGame:update(dt)
	for k, v in pairs(self.players) do
		v:update(dt)
	end
end

function ProtoGame:draw()
	for k, v in pairs(self.players) do
		v:draw()
	end
end

function ProtoGame:keypressed(key, unicode)
	--
end

function ProtoGame:keyreleased(key, unicode)
	--
end

function ProtoGame:mousepressed(x, y, button)
	--
end

function ProtoGame:mousereleased(x, y, button)
	--
end

function ProtoGame:resize(width, height)
	--
end